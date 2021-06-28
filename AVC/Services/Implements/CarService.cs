using AutoMapper;
using AVC.Constant;
using AVC.Dtos.CarDtos;
using AVC.Dtos.PagingDtos;
using AVC.Dtos.QueryFilter;
using AVC.Extensions;
using AVC.Extensions.Extensions;
using AVC.Models;
using AVC.Repositories.Interface;
using AVC.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AVC.Services.Implements
{
    public class CarService : BaseService, ICarService
    {
        public CarService(IUnitOfWork unit, IMapper mapper, IConfiguration config, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor) : base(unit, mapper, config, urlHelper, httpContextAccessor)
        {
        }

        public CarReadDto GetCarDetail(int id)
        {
            var claims = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).Claims;

            var role = claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

            Car carFromRepo = _unit.CarRepository.Get(x => x.Id == id && x.IsApproved,
                includer: x => x.Include(car => car.ManagedByNavigation)
                                .Include(car => car.AssignedCar).ThenInclude(assign => assign.Account)
                                .Include(c => c.Issue).ThenInclude(issue => issue.Type));
            if(carFromRepo == null)
            {
                throw new NotFoundException("Car Not Found");
            }
            var actorId = int.Parse(claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

            if (role.Equals(Roles.Manager))
            {
                if(carFromRepo.ManagedBy != actorId)
                {
                    throw new PermissionDeniedException("Permission Denied");
                }
            }

            if (role.Equals(Roles.Staff))
            {
                var assigned = carFromRepo.AssignedCar.FirstOrDefault(x => (bool)x.IsAvailable);
                if (assigned == null || assigned.AccountId != actorId)
                {
                    throw new PermissionDeniedException("Permission Denied");
                }
            }

            var carDto = _mapper.Map<CarReadDto>(carFromRepo);

            return carDto;
        }

        public PagingResponseDto<CarListReadDto> GetCarList(CarQueryFilter filter)
        {
            var searchValue = filter.SearchValue;
            var page = filter.Page;
            var limit = filter.Limit;
            var isAvailable = filter.IsAvailable;

            searchValue = searchValue.IsNullOrEmpty() ? "" : searchValue.Trim();

            var claims = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).Claims;

            var role = claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

            PagingDto<Car> dto = null;

            dto = _unit.CarRepository.GetAll(page, limit, x => x.Name.Contains(searchValue) && x.IsApproved,
                includer: x => x.Include(car => car.ManagedByNavigation).ThenInclude(manager => manager.Role)
                                .Include(car => car.AssignedCar).ThenInclude(assign => assign.Account));
            var actorId = int.Parse(claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

            if (role.Equals(Roles.Manager))
            {
                dto.Result = dto.Result.Where(x => x.ManagedBy == actorId);
            }

            if (role.Equals(Roles.Staff))
            {
                dto.Result = dto.Result.Where(x => x.AssignedCar.FirstOrDefault(x => (bool)x.IsAvailable).AccountId == actorId);
            }

            if (isAvailable != null)
            {
                dto.Result = dto.Result.Where(x => x.IsAvailable == isAvailable);
            }
            if (filter.IsApproved != null)
            {
                dto.Result = dto.Result.Where(x => x.IsApproved == filter.IsApproved);
            }

            var accounts = _mapper.Map<IEnumerable<CarListReadDto>>(dto.Result);

            var response = new PagingResponseDto<CarListReadDto> { Result = accounts, Count = dto.Count };

            if (limit > 0)
            {
                if ((double)dto.Count / limit > page)
                {
                    response.NextPage = _urlHelper.Link(null, new { page = page + 1, limit, searchValue });
                }

                if (page > 1)
                    response.PreviousPage = _urlHelper.Link(null, new { page = page - 1, limit, searchValue });
            }
            return response;
        }

    }
}
