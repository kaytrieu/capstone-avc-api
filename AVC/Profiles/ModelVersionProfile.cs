using AutoMapper;
using AVC.Dtos.IssueDtos;
using AVC.Dtos.ModelDtos;
using AVC.Models;

namespace AVC.Profiles
{
    public class ModelVersionProfile : Profile
    {
        public ModelVersionProfile()
        {
            CreateMap<ModelVersion, ModelReadDto>().ForMember(des => des.ModelStatus, opt => opt.MapFrom(src => src.ModelStatus.Name));
            CreateMap<ModelCreateDto, ModelVersion>().ReverseMap();
            CreateMap<ModelUpdateDto, ModelVersion>().ReverseMap();
        }

    }
}
