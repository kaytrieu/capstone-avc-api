using AutoMapper;
using AVC.Dtos.AccountDtos;
using AVC.Dtos.CarDtos;
using AVC.Models;
using System.Linq;

namespace AVC.Profiles
{
    public class CarProfile : Profile
    {
        public CarProfile()
        {
            //Source to Target
            CreateMap<Car, CarListReadDto>().ForMember(des => des.ManagedByNavigation, opt => opt.MapFrom(src => src.ManagedByNavigation))
                .ForMember(des => des.AssignedToId, opt => opt.MapFrom(src => src.AssignedCar.SingleOrDefault(assign => assign.IsAvailable == true).AccountId))
                .ForMember(des => des.AssignedToEmail, opt => opt.MapFrom(src => src.AssignedCar.SingleOrDefault(assign => assign.IsAvailable == true).Account.Email));
        }

    }
}
