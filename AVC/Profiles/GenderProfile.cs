using AutoMapper;
using AVC.Dtos.GenderDtos;
using AVC.Models;

namespace AVC.Profiles
{
    public class GenderProfile : Profile
    {
        public GenderProfile()
        {
            CreateMap<Gender, GenderReadDto>();

        }

    }
}
