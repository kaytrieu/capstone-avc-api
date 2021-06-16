using AutoMapper;
using AVC.Dtos.AccountDtos;
using AVC.Dtos.ProfileDtos;
using AVC.Models;

namespace AVC.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            //Source to Target
            CreateMap<Account, AccountReadAfterAuthenDto>();
            CreateMap<AccountStaffCreateDto, Account>();
            CreateMap<AccountStaffCreateDtoFormWrapper, AccountStaffCreateDto>();
            CreateMap<AccountManagerCreateDtoFormWrapper, AccountStaffCreateDto>();
            CreateMap<AccountManagerCreateDto, Account>();
            CreateMap<AccountUpdateDto, Account>();
            CreateMap<Account, AccountUpdateDto>();
            CreateMap<AccountActivationDto, Account>();
            CreateMap<ProfilePasswordUpdateDto, Account>()
                .ForMember(des => des.Password, opt => opt.MapFrom(src => src.NewPassword));
            CreateMap<Account, AccountManagerReadDto>()
               .ForMember(des => des.Role, opt => opt.MapFrom(src => src.Role.Name));
            CreateMap<Account, AccountStaffReadDto>()
                .ForMember(des => des.Role, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(des => des.ManagedByEmail, opt => opt.MapFrom(src => src.ManagedByNavigation.Email));
            CreateMap<Account, ProfileReadDto>()
                .ForMember(des => des.Role, opt => opt.MapFrom(src => src.Role.Name));
            CreateMap<ProfileUpdateDto, Account>();
        }

    }
}
