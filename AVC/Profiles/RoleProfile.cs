using AutoMapper;
using AVC.Dtos.RoleDtos;
using AVC.Models;

namespace AVC.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, RoleReadDto>();

        }

    }
}
