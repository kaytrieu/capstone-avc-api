using AVC.Dtos.AccountDtos;
using AVC.Dtos.PagingDtos;
using AVC.Dtos.QueryFilter;
using Morcatko.AspNetCore.JsonMergePatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AVC.Services.Interfaces
{
    public interface IAccountService
    {
        PagingResponseDto<AccountStaffReadDto> GetStaffList(AccountQueryFilter filter);
        PagingResponseDto<AccountManagerReadDto> GetManagerList(AccountQueryFilter filter);
        AccountManagerReadDto GetManagerDetail(int id);
        AccountStaffReadDto GetStaffDetail(int id);

        AccountManagerReadDto CreateManager(AccountManagerCreateDtoFormWrapper accountCreateDtoWrapper);
        AccountStaffReadDto CreateStaff(AccountStaffCreateDtoFormWrapper accountCreateDtoWrapper);
        void SetActivation(int id, AccountActivationDto accountActivationDto);
        void SetManagedBy(AccountManagedByUpdateDto dto);
        void Patch(int id, JsonMergePatchDocument<AccountUpdateDto> dto);

    }
}
