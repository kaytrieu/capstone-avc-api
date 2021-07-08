using AVC.Dtos.CarDtos;
using AVC.Dtos.HubMessages;
using AVC.Dtos.PagingDtos;
using AVC.Dtos.QueryFilter;
using Microsoft.AspNetCore.Http;
using Morcatko.AspNetCore.JsonMergePatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Services.Interfaces
{
    public interface ICarService
    {
        PagingResponseDto<CarListReadDto> GetCarList(CarQueryFilter filter);
        CarReadDto GetCarDetail(int id);
        void CreateNewCarByDevice(string deviceId);
        void SetManagedBy(CarManagedByUpdateDto dto);
        void RegisterNewCar(int id, CarApprovalDto formDto);
        void SetActivation(int id, CarActivationDto dto);
        void Update(int id, CarUpdateDto formDto);
        void AssignCar(int carId, int? staffId);
        void UpdateImage(int id, IFormFile image);
        void UpdateConfig(int id, IFormFile config);
        DefaultCarConfigDto GetDefaultCarConfig();
        void UpdateDefaultConfig(IFormFile config);
        CarConnectedMessage HandleCarConnected(string deviceId);
    }
}
