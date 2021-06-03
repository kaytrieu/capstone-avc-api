using AutoMapper;
using AVC.Constant;
using AVC.Dtos.AccountDtos;
using AVC.Dtos.PagingDtos;
using AVC.Extensions.Extensions;
using AVC.GenericRepository;
using AVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly IAccountRepository _repository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountRepository repository, IMapper mapper, IConfiguration config, ILogger<AccountsController> logger, IRoleRepository roleRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _config = config;
            _logger = logger;
            _roleRepository = roleRepository;
        }

        // GET: api/Accounts/staffs
        [Authorize]
        [HttpGet("staffs")]
        [AuthorizeRoles(Roles.Admin, Roles.Manager)]
        public ActionResult<PagingResponseDto<AccountReadDto>> GetStaffAccounts(int page = 1, int limit = 10, string searchValue = "")
        {
            searchValue = searchValue.IsNullOrEmpty() ? "" : searchValue.Trim();

            var claims = (HttpContext.User.Identity as ClaimsIdentity).Claims;

            var role = claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

            var staffRoleId = _roleRepository.Get(x => x.Name.Equals(Roles.Staff)).Id;

            PagingDto<Account> dto = _repository.GetAllWithOrderedDecs(page, limit, x => x.RoleId == staffRoleId && (x.LastName.Contains(searchValue) || x.FirstName.Contains(searchValue)),x => x.CreatedBy, x => x.Role, x => x.Gender);

            var accounts = _mapper.Map<IEnumerable<AccountReadDto>>(dto.Result);

            var response = new PagingResponseDto<AccountReadDto> { Result = accounts, Count = dto.Count };

            if (limit > 0)
            {
                if ((double)dto.Count / limit > page)
                {
                    response.NextPage = Url.Link(null, new { page = page + 1, limit, searchValue });
                }

                if (page > 1)
                    response.PreviousPage = Url.Link(null, new { page = page - 1, limit, searchValue });
            }

            return Ok(response);
        }

        //[Authorize]
        //[HttpDelete("{id}")]
        //[AuthorizeRoles(Roles.Admin, Roles.Manager)]
        //public ActionResult DeactivateAccount(int id)
        //{
        //    Account accountFromRepo = _repository.Get(x => x.Id == id);

        //    if (accountFromRepo == null)
        //    {
        //        return NotFound();
        //    }

        //    _repository.Delete(accountFromRepo);

        //    _repository.SaveChanges();

        //    return NoContent();
        //}

        // GET: api/Accounts/manager
        [Authorize]
        [HttpGet("managers")]
        [AuthorizeRoles(Roles.Admin)]
        public ActionResult<PagingResponseDto<AccountReadDto>> GetManagerAccounts(int page = 1, int limit = 10, string searchValue = "")
        {
            searchValue = searchValue.IsNullOrEmpty() ? "" : searchValue.Trim();

            var claims = (HttpContext.User.Identity as ClaimsIdentity).Claims;

            var role = claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

            var managerRoleId = _roleRepository.Get(x => x.Name.Equals(Roles.Manager)).Id;

            PagingDto<Account> dto = _repository.GetAll(page, limit, x => x.RoleId == managerRoleId && x.IsAvailable == true && (x.LastName.Contains(searchValue) || x.FirstName.Contains(searchValue)), x => x.Role, x => x.Gender);

            var accounts = _mapper.Map<IEnumerable<AccountReadDto>>(dto.Result);

            var response = new PagingResponseDto<AccountReadDto> { Result = accounts, Count = dto.Count };

            if (limit > 0)
            {
                if ((double)dto.Count / limit > page)
                {
                    response.NextPage = Url.Link(null, new { page = page + 1, limit, searchValue });
                }

                if (page > 1)
                    response.PreviousPage = Url.Link(null, new { page = page - 1, limit, searchValue });
            }

            return Ok(response);
        }

        [AuthorizeRoles(Roles.Admin, Roles.Manager)]
        [HttpGet("{id}")]
        public ActionResult<AccountReadDto> GetAccount(int id)
        {
            var claims = (HttpContext.User.Identity as ClaimsIdentity).Claims;

            var role = claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

            Account account = _repository.Get(x => x.Id == id && x.IsAvailable == true, x => x.Role, x => x.Gender);

            if (role == Roles.Admin)
            {
                if (!(account.Role.Name.Equals(Roles.Manager) || account.Role.Name.Equals(Roles.Staff)))
                {
                    return Forbid("Permission Denied");
                }
            }

            if (role == Roles.Manager)
            {
                if (!(account.Role.Name.Equals(Roles.Staff)))
                {
                    return Forbid("Permission Denied");
                }
            }

            if (account == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AccountReadDto>(account));
        }

        // POST: api/Accounts
        //store tạo staff, super tạo store
        [AuthorizeRoles(Roles.Admin, Roles.Manager)]
        [HttpPost]
        public ActionResult<AccountReadDto> PostAccount(AccountCreateDto accountCreateDto)
        {
            var claims = (HttpContext.User.Identity as ClaimsIdentity).Claims;

            var role = claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

            var roleName = _roleRepository.Get(x => x.Id == accountCreateDto.RoleId).Name;

            if (role == Roles.Admin)
            {
                if (!(roleName == Roles.Manager))
                {
                    return Forbid("Permission Denied");
                }
            }

            if (role == Roles.Manager)
            {
                if (!(roleName == Roles.Staff))
                {
                    return Forbid("Permission Denied");
                }
            }

            var actorId = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            accountCreateDto.SetCreatedById(int.Parse(actorId));

            Account accountModel = _mapper.Map<Account>(accountCreateDto);

            _repository.Add(accountModel);

            try
            {
                _repository.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.ToString().Contains("duplicate"))
                {
                    return Conflict("Existed Email");
                }
                else
                {
                    throw ex;
                }
            }
            accountModel = _repository.Get(x => x.Id == accountModel.Id, x => x.Role, x => x.Gender);

            AccountReadDto accountReadDto = _mapper.Map<AccountReadDto>(accountModel);

            return Ok();
            //return CreatedAtAction("GetAccount", new { id = accountReadDto.Id }, accountReadDto);

        }

        //[HttpPatch("{id}")]
        //public IActionResult PatchAccount(int id, JsonPatchDocument<AccountUpdateDto> patchDoc)
        //{
        //    Account accountModelFromRepo = _repository.Get(x => x.Id == id);

        //    if (accountModelFromRepo == null)
        //    {
        //        return NotFound();
        //    }

        //    AccountUpdateDto accountToPatch = _mapper.Map<AccountUpdateDto>(accountModelFromRepo);

        //    patchDoc.ApplyTo(accountToPatch, ModelState);

        //    if (!TryValidateModel(accountToPatch))
        //    {
        //        return ValidationProblem(ModelState);
        //    }

        //    //Update the DTO to repo
        //    _mapper.Map(accountToPatch, accountModelFromRepo);

        //    //Temp is not doing nothing
        //    _repository.Update(accountModelFromRepo);

        //    _repository.SaveChanges();

        //    return NoContent();
        //}
    }
}
