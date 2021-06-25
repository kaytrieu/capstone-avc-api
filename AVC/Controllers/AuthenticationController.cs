using AutoMapper;
using AVC.Dtos.AuthenticationDtos;
using AVC.Dtos.ReponseDtos;
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
        private readonly IAuthenticateService _authenticateService;

        public AuthenticationController(IAccountRepository repository, IMapper mapper, IConfiguration config, IEmailSender emailSender, IAuthenticateService authenticateService)
        {
            _repository = repository;
            _mapper = mapper;
            _config = config;
            _emailSender = emailSender;
            _authenticateService = authenticateService;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="dto">username and password</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<AuthenticationReadDto> Login([FromBody] AuthenticationPostDto dto)
        {
            var authenticationReadDto = _authenticateService.CheckLogin(dto);
            return Ok(authenticationReadDto);
        }

        #region reset password
        /// <summary>
        /// Request to send the reset password
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("reset")]
        public IActionResult GetEmail(string email)
        {
            _authenticateService.SendResetPasswordEmail(email);

            return Ok(new ResponseDto("Please check your email for password reset instructions"));
        }

        /// <summary>
        /// Change password for owner account
        /// </summary>
        /// <param name="newPasswordDto"></param>
        /// <returns></returns>
        [HttpPost("new-password")]
        public IActionResult SetNewPassword(NewPasswordDto newPasswordDto)
        {
            _authenticateService.SetNewPassword(newPasswordDto);

            return NoContent();
        }
        #endregion

    }
}
