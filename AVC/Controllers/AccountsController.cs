using AutoMapper;
using AVC.Constant;
using AVC.Dtos.AccountDtos;
using AVC.Dtos.PagingDtos;
using AVC.Dtos.QueryFilter;
using AVC.Dtos.ReponseDtos;
using AVC.Extensions.Extensions;
using AVC.Models;
using AVC.Repositories.Interface;
using AVC.Service;
using AVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Morcatko.AspNetCore.JsonMergePatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using static AVC.Extensions.Extensions.Extensions;

namespace AVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController( IAccountService accountService)
        {
            _accountService = accountService;
        }

        // GET: api/Accounts/staffs
        /// <summary>
        /// Get list of staff
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("staffs")]
        [AuthorizeRoles(Roles.Admin, Roles.Manager)]
        public ActionResult<PagingResponseDto<AccountStaffReadDto>> GetStaffAccounts([FromQuery] AccountQueryFilter filter)
        {
            var claims = (HttpContext.User.Identity as ClaimsIdentity).Claims;

            var response = _accountService.GetStaffList(filter);

            return Ok(response);
        }


        // GET: api/Accounts/manager
        /// <summary>
        /// Get List of Manager
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("managers")]
        [AuthorizeRoles(Roles.Admin)]
        public ActionResult<PagingResponseDto<AccountManagerReadDto>> GetManagerAccounts([FromQuery] AccountQueryFilter filter)
        {
            var claims = (HttpContext.User.Identity as ClaimsIdentity).Claims;

            var response = _accountService.GetManagerList(filter);

            return Ok(response);
        }

        //POST api/accounts/manager/id
        /// <summary>
        /// Get Specific Account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Admin)]
        [HttpGet("manager/{id}")]
        public ActionResult<AccountManagerDetailReadDto> GetManagerAccount(int id)
        {
            var account = _accountService.GetManagerDetail(id);

            return Ok(account);
        }

        /// <summary>
        /// Get Specific Account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Admin, Roles.Manager)]
        [HttpGet("staff/{id}")]
        public ActionResult<AccountStaffDetailReadDto> GetStaffAccount(int id)
        {
            var account = _accountService.GetStaffDetail(id);

            return Ok(account);
        }

        // POST: api/Accounts
        /// <summary>
        /// Create new Account
        /// </summary>
        /// <param name="accountCreateDtoWrapper"></param>
        /// <returns>Account for success, 401 for permission denied, 409 for email conflict</returns>
        [AuthorizeRoles(Roles.Admin)]
        [HttpPost("manager")]
        public ActionResult<AccountManagerReadDto> PostManager([FromForm] AccountManagerCreateDtoFormWrapper accountCreateDtoWrapper)
        {
            AccountManagerReadDto accountReadDto = _accountService.CreateManager(accountCreateDtoWrapper);

            //return Ok();
            return CreatedAtAction("GetStaffAccount", new { id = accountReadDto.Id }, accountReadDto);
        }

        // POST: api/Accounts
        /// <summary>
        /// Create new Account
        /// </summary>
        /// <param name="accountCreateDtoWrapper"></param>
        /// <returns>Account for success, 401 for permission denied, 409 for email conflict</returns>
        [AuthorizeRoles(Roles.Admin)]
        [HttpPost("staff")]
        public ActionResult<AccountStaffReadDto> PostStaffAccount([FromForm] AccountStaffCreateDtoFormWrapper accountCreateDtoWrapper)
        {
           
            AccountStaffReadDto accountReadDto = _accountService.CreateStaff(accountCreateDtoWrapper);

            //return Ok();
            return CreatedAtAction("GetStaffAccount", new { id = accountReadDto.Id }, accountReadDto);
        }

        /// <summary>
        /// Activate or Deactivate account
        /// </summary>
        /// <param name="id">Id of Account</param>
        /// <param name="accountActivationDto">IsAvailable: True for activate, false for deactivate</param>
        /// <returns>Http Status, 204 for success</returns>
        [AuthorizeRoles(Roles.Admin, Roles.Manager)]
        [HttpPut("{id}/activation")]
        public ActionResult SetActivation(int id, AccountActivationDto accountActivationDto)
        {
            _accountService.SetActivation(id, accountActivationDto);

            return Ok();
        }


        // PUT api/accounts/managedBy
        /// <summary>
        /// Assign/UnAssign Staff for Manager
        /// </summary>
        /// <param name="dto">activate true or false</param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Admin)]
        [HttpPut("managedBy")]
        public ActionResult SetManagedBy(AccountManagedByUpdateDto dto)
        {
            _accountService.SetManagedBy(dto);

            return Ok();
        }

        //Patch
        /// <summary>
        /// Partitle update account
        /// </summary>
        /// <param name="id">id of account</param>
        /// <param name="dto">update dto</param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [AuthorizeRoles(Roles.Admin)]
        [Consumes(JsonMergePatchDocument.ContentType)]
        public IActionResult PatchAccount(int id, [FromBody] JsonMergePatchDocument<AccountUpdateDto> dto)
        {
            _accountService.Patch(id, dto);

            return NoContent();
        }

    }
}
