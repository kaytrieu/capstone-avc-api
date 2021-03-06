using AutoMapper;
using AVC.Dtos.CarDtos;
using AVC.Dtos.HubMessages;
using AVC.Extensions.Extensions;
using AVC.Models;
using System.Linq;

namespace AVC.Profiles
{
    public class CarProfile : Profile
    {
        public CarProfile()
        {
            //Source to Target
            CreateMap<Car, CarListReadDto>().ForMember(des => des.ManagedBy, opt => opt.MapFrom(src => src.ManagedByNavigation))
                .ForMember(des => des.AssignTo, opt => opt.MapFrom(src => src.AssignedCar.SingleOrDefault(assign => assign.IsAvailable == true).Account));
            CreateMap<Car, CarReadDto>().ForMember(des => des.ManagedBy, opt => opt.MapFrom(src => src.ManagedByNavigation))
                .ForMember(des => des.AssignTo, opt => opt.MapFrom(src => src.AssignedCar.SingleOrDefault(assign => assign.IsAvailable == true).Account))
                .ForMember(des => des.Issues, opt => opt.MapFrom(src => src.Issue));
            CreateMap<Car, CarAssignedReadDto>();
            CreateMap<Car, CarMessageDto>();
            CreateMap<CarApprovalDto, Car>();
            CreateMap<Car, CarApprovalDto>().Ignore(car => car.ImageFile);
            CreateMap<DefaultConfiguration, DefaultCarConfigDto>();
        }

    }
}
