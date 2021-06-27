using AutoMapper;
using AVC.Dtos.IssueDtos;
using AVC.Models;

namespace AVC.Profiles
{
    public class IssuelProfile : Profile
    {
        public IssuelProfile()
        {
            CreateMap<Issue, IssueReadDto>().ForMember(des => des.Type, opt => opt.MapFrom(src => src.Type.Name));
            CreateMap<IssueCreateDto, Issue>().ReverseMap();
            CreateMap<IssueType, TypeReadDto>();
        }

    }
}
