using AutoMapper;
using AVC.Dtos.AuthenticationDtos;
using AVC.GenericRepository;
using AVC.Models;
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
                return Unauthorized("Invalid Email or Password");
            }

            if ((bool)!accountModel.IsAvailable)
            {
                return Unauthorized("Your Account is Deactivated");
            }

            bool isAuthorized = accountModel.Password.Equals(BCrypt.Net.BCrypt.HashPassword(dto.Password, accountModel.Salt));

            if (!isAuthorized)
            {
                return Unauthorized("Invalid Email or Password");
            }

            string tokenStr = GenerateJSONWebToken(accountModel);

            AuthenticationReadDto authenticationReadDto = _mapper.Map<AuthenticationReadDto>(accountModel);
            authenticationReadDto.Token = tokenStr;

            return Ok(authenticationReadDto);

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
    }
}
