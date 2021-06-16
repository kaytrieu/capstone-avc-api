using AutoMapper;
using AVC.Constant;
using AVC.Dtos.AccountDtos;
using AVC.Dtos.PagingDtos;
using AVC.Dtos.ReponseDtos;
using AVC.Extensions.Extensions;
using AVC.GenericRepository;
using AVC.Models;
using AVC.Service;
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
        /// <summary>
        /// Get list of staff
        /// </summary>
        /// <param name="page">page number</param>
        /// <param name="limit">entities number each page</param>
        /// <param name="searchValue">Search By FirstName or Lastname</param>
        /// <param name="isAvailable">Filter by account activation</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("staffs")]
        [AuthorizeRoles(Roles.Admin, Roles.Manager)]
        public ActionResult<PagingResponseDto<AccountStaffReadDto>> GetStaffAccounts(int page = 1, int limit = 10, string searchValue = "", bool? isAvailable = null)
        {
            searchValue = searchValue.IsNullOrEmpty() ? "" : searchValue.Trim();

            var claims = (HttpContext.User.Identity as ClaimsIdentity).Claims;

            var role = claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

            var staffRoleId = _roleRepository.Get(x => x.Name.Equals(Roles.Staff)).Id;

            PagingDto<Account> dto = null;

            dto = _repository.GetAllWithOrderedDecs(page, limit, x => x.RoleId == staffRoleId && (x.LastName.Contains(searchValue) || x.FirstName.Contains(searchValue)), x => x.Role, x => x.ManagedByNavigation);


            if (role.Equals(Roles.Manager))
            {
                var managerId = int.Parse(claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

                dto.Result = dto.Result.Where(x => x.ManagedBy == managerId);
                //dto = _repository.GetAllWithOrderedDecs(page, limit, x => x.RoleId == staffRoleId && x.ManagedBy == managerId && (x.LastName.Contains(searchValue) || x.FirstName.Contains(searchValue)), x => x.Role);
            }

            if (isAvailable != null)
            {
                dto.Result = dto.Result.Where(x => x.IsAvailable == isAvailable);
            }

            var accounts = _mapper.Map<IEnumerable<AccountStaffReadDto>>(dto.Result);

            var response = new PagingResponseDto<AccountStaffReadDto> { Result = accounts, Count = dto.Count };

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


        // GET: api/Accounts/manager
        /// <summary>
        /// Get List of Manager
        /// </summary>
        /// <param name="page">page number</param>
        /// <param name="limit">entity number of each page</param>
        /// <param name="searchValue">Search By FirstName or Lastname</param>
        /// <param name="isAvailable">Filter by account activation</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("managers")]
        [AuthorizeRoles(Roles.Admin)]
        public ActionResult<PagingResponseDto<AccountManagerReadDto>> GetManagerAccounts(int page = 1, int limit = 10, string searchValue = "", bool? isAvailable = null)
        {
            searchValue = searchValue.IsNullOrEmpty() ? "" : searchValue.Trim();

            var claims = (HttpContext.User.Identity as ClaimsIdentity).Claims;

            var role = claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

            var managerRoleId = _roleRepository.Get(x => x.Name.Equals(Roles.Manager)).Id;

            PagingDto<Account> dto = _repository.GetAll(page, limit, x => x.RoleId == managerRoleId && x.IsAvailable == true && (x.LastName.Contains(searchValue) || x.FirstName.Contains(searchValue)), x => x.Role);

            if (isAvailable != null)
            {
                dto.Result = dto.Result.Where(x => x.IsAvailable == isAvailable);
            }

            var accounts = _mapper.Map<IEnumerable<AccountManagerReadDto>>(dto.Result);

            var response = new PagingResponseDto<AccountManagerReadDto> { Result = accounts, Count = dto.Count };

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

        //POST api/accounts/manager/id
        /// <summary>
        /// Get Specific Account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Admin)]
        [HttpGet("manager/{id}")]
        public ActionResult<AccountManagerReadDto> GetManagerAccount(int id)
        {
            var claims = (HttpContext.User.Identity as ClaimsIdentity).Claims;

            var role = claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

            Account account = _repository.Get(x => x.Id == id && x.Role.Name.Equals(Roles.Manager), x => x.Role);

            if (account == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorResponseDto("Can not found account."));
            }

            return Ok(_mapper.Map<AccountManagerReadDto>(account));
        }

        /// <summary>
        /// Get Specific Account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Admin, Roles.Manager)]
        [HttpGet("staff/{id}")]
        public ActionResult<AccountManagerReadDto> GetStaffAccount(int id)
        {
            var claims = (HttpContext.User.Identity as ClaimsIdentity).Claims;

            var role = claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

            Account account = _repository.Get(x => x.Id == id && x.Role.Name.Equals(Roles.Staff), x => x.Role, x => x.ManagedByNavigation);

            if (account == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorResponseDto("Can not found account."));
            }

            if (role == Roles.Manager)
            {
                var managerId = int.Parse(claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

                if (account.ManagedBy != managerId)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponseDto("Permission Denied"));
                }
            }

            return Ok(_mapper.Map<AccountManagerReadDto>(account));
        }

        // POST: api/Accounts
        /// <summary>
        /// Create new Account
        /// </summary>
        /// <param name="accountCreateDtoWrapper"></param>
        /// <returns>Account for success, 401 for permission denied, 409 for email conflict</returns>
        [AuthorizeRoles(Roles.Admin)]
        [HttpPost("manager")]
        public ActionResult<AccountManagerReadDto> PostManager(AccountManagerCreateDtoFormWrapper accountCreateDtoWrapper)
        {
            var accountCreateDto = _mapper.Map<AccountStaffCreateDto>(accountCreateDtoWrapper);

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
                    return StatusCode(StatusCodes.Status409Conflict, new ErrorResponseDto("Existed Email"));
                }
                else
                {
                    throw ex;
                }
            }

            accountModel.Avatar = UploadAvatar(accountCreateDtoWrapper.AvatarImage, accountModel.Id);

            _repository.SaveChanges();

            accountModel = _repository.Get(x => x.Id == accountModel.Id, x => x.Role, x => x.ManagedByNavigation);

            AccountStaffReadDto accountReadDto = _mapper.Map<AccountStaffReadDto>(accountModel);

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
            var accountCreateDto = _mapper.Map<AccountStaffCreateDto>(accountCreateDtoWrapper);

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
                    return StatusCode(StatusCodes.Status409Conflict, new ErrorResponseDto("Existed Email"));
                }
                else
                {
                    throw ex;
                }
            }

            accountModel.Avatar = UploadAvatar(accountCreateDtoWrapper.AvatarImage, accountModel.Id);

            _repository.SaveChanges();

            accountModel = _repository.Get(x => x.Id == accountModel.Id, x => x.Role, x => x.ManagedByNavigation);

            AccountStaffReadDto accountReadDto = _mapper.Map<AccountStaffReadDto>(accountModel);

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
            var claims = (HttpContext.User.Identity as ClaimsIdentity).Claims;
            var role = claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

            Account account = _repository.Get(x => x.Id == id, x => x.Role, x => x.ManagedByNavigation, x => x.InverseManagedByNavigation, x => x.Car);

            if (account == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorResponseDto("Account not found"));
            }

            if (role == Roles.Admin)
            {
                if (!(account.Role.Name.Equals(Roles.Manager) || account.Role.Name.Equals(Roles.Staff)))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponseDto("Permission Denied"));
                }
            }

            if (role == Roles.Manager)
            {
                if (!(account.Role.Name.Equals(Roles.Staff)))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponseDto("Permission Denied"));
                }
            }

            if (account == null)
            {
                return NotFound(new ErrorResponseDto("Can not found account."));
            }

            if (!accountActivationDto.IsAvailable)
            {
                if (account.Role.Name.Equals(Roles.Manager))
                {
                    //get all staff managed by this manager
                    var members = account.InverseManagedByNavigation;
                    if (members.Count != 0)
                    {
                        //set staff managed by nobody
                        foreach (var item in members)
                        {
                            item.ManagedBy = null;
                        }
                        //disable assign car for all staff managed by this manager
                        //set Car's managed by nobody
                        foreach (var item in account.Car)
                        {
                            item.ManagedBy = null;
                            foreach (var assigned in item.AssignedCar.Where(assign => assign.IsAvailable == true))
                            {
                                assigned.IsAvailable = false;
                                assigned.RemoveAt = DateTime.UtcNow.AddHours(7);
                            }
                        }
                    }
                }
                else
                {
                    //account.ManagedBy = null;
                    //disable control car permission by this staff
                    var assignedList = account.AssignedCarAccount.Where(x => x.IsAvailable == true);

                    foreach (var assign in assignedList)
                    {
                        assign.IsAvailable = false;
                        assign.RemoveAt = DateTime.UtcNow.AddHours(7);
                    }
                }
            }

            //Mapper to Update new password and salt
            _mapper.Map(accountActivationDto, account);

            _repository.Update(account);

            _repository.SaveChanges();


            return NoContent();
        }

        private string UploadAvatar(IFormFile image, int id)
        {
            string imageUrl = string.Empty;

            if (image != null && image.Length > 0)
            {
                imageUrl = FirebaseService.UploadFileToFirebaseStorage(image.OpenReadStream(), ("Account" + id).GetHashString(), "Avatar", _config).Result;
            }

            return imageUrl;
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
            var account = _repository.Get(x => x.Id == dto.StaffId, x => x.AssignedCarAccount);

            if (account == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorResponseDto("Staff not found"));
            }

            if (account.ManagedBy != null && account.ManagedBy != dto.ManagerId)
            {
                var assignedList = account.AssignedCarAccount.Where(x => x.IsAvailable == true);

                foreach (var assign in assignedList)
                {
                    assign.IsAvailable = false;
                    assign.RemoveAt = DateTime.UtcNow.AddHours(7);
                }
            }

            account.ManagedBy = dto.ManagerId;

            _repository.SaveChanges();

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
            Account accountModelFromRepo = _repository.Get(x => x.Id == id);

            if (accountModelFromRepo == null)
            {
                return NotFound();
            }

            AccountUpdateDto accountToPatch = _mapper.Map<AccountUpdateDto>(accountModelFromRepo);

            dto.ApplyTo(accountToPatch);

            _mapper.Map(accountToPatch, accountModelFromRepo);

            //Temp is not doing nothing
            _repository.Update(accountModelFromRepo);

            _repository.SaveChanges();

            return NoContent();
        }

    }
}
