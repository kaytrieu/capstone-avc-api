using AutoMapper;
using AVC.Dtos.AuthenticationDtos;
using AVC.Dtos.ReponseDtos;
using AVC.Extensions;
using AVC.Models;
using AVC.Repositories.Interface;
using AVC.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tagent.EmailService;
using Tagent.EmailService.Define;

namespace AVC.Services.Implements
{
    public class AuthenticateService : BaseService, IAuthenticateService
    {
        private readonly IEmailSender _emailSender;

        public AuthenticateService(IUnitOfWork unit, IMapper mapper, IConfiguration config, IEmailSender emailSender, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor)
        : base(unit, mapper, config, urlHelper, httpContextAccessor)
        {
            _emailSender = emailSender;
        }

        public AuthenticationReadDto CheckLogin(AuthenticationPostDto dto)
        {
            Account accountModel = _unit.AccountRepository.Get(x => x.Email == dto.Email, x => x.Role, x => x.ManagedByNavigation);

            if (accountModel == null)
            {
                throw new UnauthorizedAccessException("Invalid Email or Password");
            }

            bool isAuthorized = accountModel.Password.Equals(BCrypt.Net.BCrypt.HashPassword(dto.Password, accountModel.Salt));

            if (!isAuthorized)
            {
                throw new UnauthorizedAccessException("Invalid Email or Password");
            }

            if ((bool)!accountModel.IsAvailable)
            {
                throw new UnauthorizedAccessException("Account deactivated");

            }
            string tokenStr = GenerateJSONWebToken(accountModel);

            AuthenticationReadDto authenticationReadDto = _mapper.Map<AuthenticationReadDto>(accountModel);
            authenticationReadDto.Token = tokenStr;

            return authenticationReadDto;
        }

        private string GenerateJSONWebToken(Account accountModel)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.Role, accountModel.Role.Name),
                new Claim(ClaimTypes.Email, accountModel.Email),
                new Claim(ClaimTypes.Name, accountModel.FirstName + " " + accountModel.LastName),
                new Claim(ClaimTypes.NameIdentifier, accountModel.Id.ToString())
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims,
                expires: null,
                signingCredentials: credentials

                );
            string encodeToken = new JwtSecurityTokenHandler().WriteToken(token);

            return encodeToken;

        }

        private void SendMail(string securityKey, Account account)
        {
            var email = account.Email;
            List<string> listemail = new List<string>
            {
                email
            };
            string linkEmail = _config.GetValue<String>("ResetPasswordLink") + email;
            string linkLogo = _config.GetValue<String>("LogoLink");
            var template = Properties.Resources.password_reset
                .Replace("[account_name]", account.FirstName + " " + account.LastName)
                .Replace("[security_key]", securityKey)
                .Replace("[reset_password_button]", linkEmail)
                .Replace("[reset_password_link]", linkEmail)
                .Replace("[link_to_logo]", linkLogo);
            var message = new Message(listemail, "[AVC System] Reset Your Password", template);
            _emailSender.SendEmail(message);
        }

        public void SendResetPasswordEmail(string email)
        {
            var account = _unit.AccountRepository.Get(x => x.Email.Equals(email));
            if (account == null || account.Password.Equals(""))
            {
                throw new NotFoundException("Invalid email");
            }

            Random generator = new Random();
            var securityKey = generator.Next(0, 1000000).ToString("D6");

            account.ResetPasswordToken = securityKey;
            _unit.AccountRepository.SaveChanges();

            SendMail(securityKey, account);
        }

        public void SetNewPassword(NewPasswordDto newPasswordDto)
        {
            Account accountModel = _unit.AccountRepository.Get(x => x.Email == newPasswordDto.Email);

            if (accountModel == null)
            {
                throw new NotFoundException("Account not found");
            }

            bool isAuthorized = accountModel.ResetPasswordToken.Equals(newPasswordDto.SecurityKey);

            accountModel.ResetPasswordToken = "";

            if (!isAuthorized)
            {
                _unit.SaveChanges();
                throw new PermissionDeniedException("Security key is not correct");
            }

            //Mapper to Update new password and salt
            _mapper.Map(newPasswordDto, accountModel);

            _unit.AccountRepository.Update(accountModel);

            _unit.SaveChanges();
        }
    }
}
