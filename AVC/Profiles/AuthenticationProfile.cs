using AutoMapper;
using AVC.Dtos.AuthenticationDtos;
using AVC.Models;

namespace AVC.Profiles
{
    public class AuthenticationProfile : Profile
    {
        public AuthenticationProfile()
        {
            CreateMap<Account, AuthenticationReadDto>()
                .ForMember(dest => dest.Account, opt => opt.MapFrom(src => src));
            CreateMap<NewPasswordDto, Account>()
                .ForMember(dest => dest.Email, src => src.Ignore());

        }

    }
}
