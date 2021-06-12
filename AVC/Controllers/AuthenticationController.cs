using AutoMapper;
using AVC.Dtos.AuthenticationDtos;
using AVC.Dtos.ReponseDtos;
using AVC.GenericRepository;
using AVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Tagent.EmailService;
using Tagent.EmailService.Define;

namespace AVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAccountRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IEmailSender _emailSender;

        public AuthenticationController(IAccountRepository repository, IMapper mapper, IConfiguration config, IEmailSender emailSender)
        {
            _repository = repository;
            _mapper = mapper;
            _config = config;
            _emailSender = emailSender;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="dto">username and password</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<AuthenticationReadDto> Login([FromBody] AuthenticationPostDto dto)
        {
            Account accountModel = _repository.Get(x => x.Email == dto.Email, x => x.Role);

            if (accountModel == null)
            {
                return Unauthorized(new ErrorResponseDto("Invalid Email or Password"));
            }

            bool isAuthorized = accountModel.Password.Equals(BCrypt.Net.BCrypt.HashPassword(dto.Password, accountModel.Salt));

            if (!isAuthorized)
            {
                return Unauthorized(new ErrorResponseDto("Invalid Email or Password"));
            }

            if ((bool)!accountModel.IsAvailable)
            {
                return Unauthorized(new ErrorResponseDto("Your Account is Deactivated"));
            }
            string tokenStr = GenerateJSONWebToken(accountModel);

            AuthenticationReadDto authenticationReadDto = _mapper.Map<AuthenticationReadDto>(accountModel);
            authenticationReadDto.Token = tokenStr;

            return Ok(authenticationReadDto);
        }

        #region reset password
        [HttpPost("reset")]
        public IActionResult GetEmail(string email)
        {
            var account = _repository.Get(x => x.Email.Equals(email));
            if (account == null || account.Password.Equals(""))
            {
                return StatusCode(404, new ErrorResponseDto("Invalid Email"));
            }

            Random generator = new Random();
            var securityKey = generator.Next(0, 1000000).ToString("D6");

            account.ResetPasswordToken = securityKey;
            _repository.SaveChanges();

            var resource = SendMail(securityKey, account);
            if (resource.Equals("Success"))
            {
                return Ok(new ErrorResponseDto("Please check your email for password reset instructions"));
            }
            else
            {
                return StatusCode(500, resource);
            }
        }

        /// <summary>
        /// Change password for owner account
        /// </summary>
        /// <param name="newPasswordDto"></param>
        /// <returns></returns>
        [HttpPost("new-password")]
        public IActionResult SetNewPassword(NewPasswordDto newPasswordDto)
        {
            Account accountModel = _repository.Get(x => x.Email == newPasswordDto.Email);

            if (accountModel == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorResponseDto("Account Not Found"));
            }

            bool isAuthorized = accountModel.ResetPasswordToken.Equals(newPasswordDto.SecurityKey);

            accountModel.ResetPasswordToken = "";

            if (!isAuthorized)
            {
                _repository.SaveChanges();
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponseDto("Security Key is not correct"));
            }

            //Mapper to Update new password and salt
            _mapper.Map(newPasswordDto, accountModel);

            _repository.Update(accountModel);

            _repository.SaveChanges();


            return NoContent();
        }

        private string SendMail(string securityKey, Account account)
        {
            var email = account.Email;
            List<string> listemail = new List<string>
            {
                email
            };
            string linkEmail = _config.GetValue<String>("ResetPasswordLink");
            string linkLogo = _config.GetValue<String>("LogoLink");
            var template = Properties.Resources.password_reset
                .Replace("[account_name]",account.FirstName + account.LastName)
                .Replace("[security_key]",securityKey)
                .Replace("[reset_password_link]",linkEmail)
                .Replace("[link_to_logo]", linkLogo);
            var message = new Message(listemail, "[AVC System] Reset Your Password", template);
            _emailSender.SendEmail(message);
            return "Success";

        }


        #endregion

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
    }
}
