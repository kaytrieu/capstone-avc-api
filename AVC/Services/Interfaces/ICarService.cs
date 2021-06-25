using AVC.Dtos.CarDtos;
using AVC.Dtos.PagingDtos;
using AVC.Dtos.QueryFilter;
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

    }
}
