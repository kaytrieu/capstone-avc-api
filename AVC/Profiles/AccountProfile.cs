using AutoMapper;
using AVC.Dtos.AccountDtos;
using AVC.Models;

namespace AVC.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            //Source to Target
            CreateMap<Account, AccountReadAfterAuthenDto>();
            CreateMap<AccountCreateDto, Account>();
            CreateMap<Account, AccountReadDto>()
                .ForMember(des => des.Role, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(des => des.Gender, opt => opt.MapFrom(src => src.Gender.Name));
        }

    }
}
