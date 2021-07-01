using AutoMapper;
using AVC.Dtos.AccountDtos;
using AVC.Dtos.ProfileDtos;
using AVC.Models;
using System.Linq;

namespace AVC.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            //Source to Target
            CreateMap<AccountStaffCreateDto, Account>();
            CreateMap<AccountStaffCreateDtoFormWrapper, AccountStaffCreateDto>();
            CreateMap<AccountManagerCreateDtoFormWrapper, AccountStaffCreateDto>();
            CreateMap<AccountManagerCreateDto, Account>();
            CreateMap<AccountUpdateDto, Account>();
            CreateMap<Account, AccountUpdateDto>();
            CreateMap<Account, AccountStaffAssignToReadDto>();
            CreateMap<AccountActivationDto, Account>();
            CreateMap<ProfilePasswordUpdateDto, Account>()
                .ForMember(des => des.Password, opt => opt.MapFrom(src => src.NewPassword));
            CreateMap<Account, AccountNotManagedByReadDto>()
               .ForMember(des => des.Role, opt => opt.MapFrom(src => src.Role.Name));
            CreateMap<Account, AccountManagerDetailReadDto>()
                .ForMember(des => des.Role, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(des => des.AssignedStaffs, opt => opt.MapFrom(src => src.InverseManagedByNavigation))
                .ForMember(des => des.AssignedCars, opt => opt.MapFrom(src => src.Car));
            CreateMap<Account, AccountReadDto>()
                .ForMember(des => des.Role, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(des => des.ManagedBy, opt => opt.MapFrom(src => src.ManagedByNavigation));
            CreateMap<Account, AccountStaffDetailReadDto>()
               .ForMember(des => des.Role, opt => opt.MapFrom(src => src.Role.Name))
               .ForMember(des => des.ManagedBy, opt => opt.MapFrom(src => src.ManagedByNavigation))
               .ForMember(des => des.AssignedCars, opt => opt.MapFrom(src => src.AssignedCarAccount.Where(assign => (bool)assign.IsAvailable).Select(assign => assign.Car)));
            CreateMap<ProfileUpdateDto, Account>();
        }

    }
}
