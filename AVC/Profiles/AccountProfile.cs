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
            CreateMap<AccountCreateDto, Account>();
            CreateMap<AccountActivationDto, Account>();
            CreateMap<ProfilePasswordUpdateDto, Account>()
                .ForMember(des => des.Password, opt => opt.MapFrom(src => src.NewPassword));
            CreateMap<Account, AccountReadDto>()
                .ForMember(des => des.Role, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(des => des.Gender, opt => opt.MapFrom(src => src.Gender.Name))
                .ForMember(des => des.CreatedByEmail, opt => opt.MapFrom(src => src.CreatedByNavigation.Email));
            CreateMap<Account, ProfileReadDto>()
                .ForMember(des => des.Role, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(des => des.Gender, opt => opt.MapFrom(src => src.Gender.Name));
        }

    }
}
