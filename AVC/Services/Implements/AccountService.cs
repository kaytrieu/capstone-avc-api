using AutoMapper;
using AVC.Constant;
using AVC.Dtos.AccountDtos;
using AVC.Dtos.PagingDtos;
using AVC.Dtos.QueryFilter;
using AVC.Extensions;
using AVC.Extensions.Extensions;
using AVC.Models;
using AVC.Repositories.Interface;
using AVC.Service;
using AVC.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Morcatko.AspNetCore.JsonMergePatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace AVC.Services.Implements
{
    public class AccountService : BaseService, IAccountService
    {
        public AccountService(IUnitOfWork unit, IMapper mapper, IConfiguration config, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor) : base(unit, mapper, config, urlHelper, httpContextAccessor)
        {
        }

        public PagingResponseDto<AccountStaffReadDto> GetStaffList(AccountQueryFilter filter)
        {
            var searchValue = filter.SearchValue;
            var page = filter.Page;
            var limit = filter.Limit;
            var isAvailable = filter.IsAvailable;

            var claims = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).Claims;

            searchValue = searchValue.IsNullOrEmpty() ? "" : searchValue.Trim();

            var role = claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

            var staffRoleId = _unit.RoleRepository.Get(x => x.Name.Equals(Roles.Staff)).Id;

            PagingDto<Account> dto = null;

            dto = _unit.AccountRepository.GetAll(page, limit, x => x.RoleId == staffRoleId && (x.LastName.Contains(searchValue) || x.FirstName.Contains(searchValue)), x => x.Role, x => x.ManagedByNavigation);


            if (role.Equals(Roles.Manager))
            {
                var managerId = int.Parse(claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

                dto.Result = dto.Result.Where(x => x.ManagedBy == managerId);
                //dto = _unit.AccountRepository.GetAllWithOrderedDecs(page, limit, x => x.RoleId == staffRoleId && x.ManagedBy == managerId && (x.LastName.Contains(searchValue) || x.FirstName.Contains(searchValue)), x => x.Role);
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
                    response.NextPage = _urlHelper.Link(null, new { page = page + 1, limit, searchValue, isAvailable });
                }

                if (page > 1)
                    response.PreviousPage = _urlHelper.Link(null, new { page = page - 1, limit, searchValue, isAvailable });
            }
            return response;
        }

        public PagingResponseDto<AccountManagerReadDto> GetManagerList(AccountQueryFilter filter)
        {
            var searchValue = filter.SearchValue;
            var page = filter.Page;
            var limit = filter.Limit;
            var isAvailable = filter.IsAvailable;

            searchValue = searchValue.IsNullOrEmpty() ? "" : searchValue.Trim();

            var claims = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).Claims;

            var role = claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

            var managerRoleId = _unit.RoleRepository.Get(x => x.Name.Equals(Roles.Manager)).Id;

            PagingDto<Account> dto = _unit.AccountRepository.GetAll(page, limit, x => x.RoleId == managerRoleId && (x.LastName.Contains(searchValue) || x.FirstName.Contains(searchValue)), x => x.Role);

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
                    response.NextPage = _urlHelper.Link(null, new { page = page + 1, limit, searchValue });
                }

                if (page > 1)
                    response.PreviousPage = _urlHelper.Link(null, new { page = page - 1, limit, searchValue });
            }

            return response;
        }

        public AccountManagerDetailReadDto GetManagerDetail(int id)
        {
            var claims = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).Claims;

            var role = claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

            Account account = _unit.AccountRepository.Get(x => x.Id == id && x.Role.Name.Equals(Roles.Manager), x => x.Role, 
                                                                                                                x => x.InverseManagedByNavigation,
                                                                                                                x => x.Car);

            if (account == null)
            {
                throw new NotFoundException("Account not found");
            }
                   

            return _mapper.Map<AccountManagerDetailReadDto>(account);
        }

        public AccountStaffDetailReadDto GetStaffDetail(int id)
        {
            var claims = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).Claims;

            var role = claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

            Account account = _unit.AccountRepository.Get(x => x.Id == id && x.Role.Name.Equals(Roles.Staff),
                                    includer: x => x.Include(staff => staff.Role)
                                                    .Include(x => x.ManagedByNavigation)
                                                    .Include(x => x.AssignedCarAccount).ThenInclude(assign => assign.Car));

            if (account == null)
            {
                throw new NotFoundException("Account not found");
            }

            if (role == Roles.Manager)
            {
                var managerId = int.Parse(claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

                if (account.ManagedBy != managerId)
                {
                    throw new PermissionDeniedException("Permission denied");
                }
            }
            return _mapper.Map<AccountStaffDetailReadDto>(account);
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

        public AccountManagerReadDto CreateManager(AccountManagerCreateDtoFormWrapper accountCreateDtoWrapper)
        {
            var accountCreateDto = _mapper.Map<AccountManagerCreateDto>(accountCreateDtoWrapper);

            Account accountModel = _mapper.Map<Account>(accountCreateDto);

            _unit.AccountRepository.Add(accountModel);

            try
            {
                _unit.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.ToString().Contains("duplicate"))
                {
                    throw new ConflictEntityException("Existed email");
                }
                else
                {
                    throw ex;
                }
            }

            accountModel.Avatar = UploadAvatar(accountCreateDtoWrapper.AvatarImage, accountModel.Id);

            _unit.SaveChanges();

            accountModel = _unit.AccountRepository.Get(x => x.Id == accountModel.Id, x => x.Role);

            AccountManagerReadDto accountReadDto = _mapper.Map<AccountManagerReadDto>(accountModel);

            return accountReadDto;
        }

        public AccountStaffReadDto CreateStaff(AccountStaffCreateDtoFormWrapper accountCreateDtoWrapper)
        {
            var accountCreateDto = _mapper.Map<AccountStaffCreateDto>(accountCreateDtoWrapper);

            Account accountModel = _mapper.Map<Account>(accountCreateDto);

            _unit.AccountRepository.Add(accountModel);

            try
            {
                _unit.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.ToString().Contains("duplicate"))
                {
                    throw new ConflictEntityException("Existed email");
                }
                else
                {
                    throw ex;
                }
            }

            accountModel.Avatar = UploadAvatar(accountCreateDtoWrapper.AvatarImage, accountModel.Id);

            _unit.SaveChanges();

            accountModel = _unit.AccountRepository.Get(x => x.Id == accountModel.Id, x => x.Role, x => x.ManagedByNavigation);

            return _mapper.Map<AccountStaffReadDto>(accountModel);
        }

        public void SetActivation(int id, AccountActivationDto accountActivationDto)
        {
            var claims = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).Claims;
            var role = claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

            Account account = _unit.AccountRepository.Get(x => x.Id == id, x => x.Role, x => x.ManagedByNavigation, x => x.InverseManagedByNavigation, x => x.Car);

            if (account == null)
            {
                throw new NotFoundException("Account not found");
            }

            if (role == Roles.Admin)
            {
                if (!(account.Role.Name.Equals(Roles.Manager) || account.Role.Name.Equals(Roles.Staff)))
                {
                    throw new PermissionDeniedException("Permission denied");
                }
            }

            if (role == Roles.Manager)
            {
                if (!(account.Role.Name.Equals(Roles.Staff)))
                {
                    throw new PermissionDeniedException("Permission denied");
                }
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

            _unit.AccountRepository.Update(account);

            _unit.SaveChanges();
        }

        public void SetManagedBy(AccountManagedByUpdateDto dto)
        {
            var account = _unit.AccountRepository.Get(x => x.Id == dto.StaffId, x => x.AssignedCarAccount);

            if (account == null)
            {
                throw new NotFoundException("Account not found");
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
            _unit.AccountRepository.Update(account);
            _unit.SaveChanges();
        }

        public void Patch(int id, JsonMergePatchDocument<AccountUpdateDto> dto)
        {
            Account accountModelFromRepo = _unit.AccountRepository.Get(x => x.Id == id);

            if (accountModelFromRepo == null)
            {
                throw new NotFoundException("Account not found");
            }

            AccountUpdateDto accountToPatch = _mapper.Map<AccountUpdateDto>(accountModelFromRepo);

            dto.ApplyTo(accountToPatch);

            if (accountToPatch.RoleId == Roles.AdminId)
            {
                throw new NotFoundException("Role not found");
            }

            _mapper.Map(accountToPatch, accountModelFromRepo);

            //Temp is not doing nothing
            _unit.AccountRepository.Update(accountModelFromRepo);

            _unit.SaveChanges();
        }
    }
}
