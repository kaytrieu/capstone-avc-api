using AutoMapper;
using AVC.Constant;
using AVC.Dtos.CarDtos;
using AVC.Dtos.PagingDtos;
using AVC.Dtos.QueryFilter;
using AVC.Extensions;
using AVC.Extensions.Extensions;
using AVC.Models;
using AVC.Repositories.Interface;
using AVC.Service;
using AVC.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Morcatko.AspNetCore.JsonMergePatch;
using Morcatko.AspNetCore.JsonMergePatch.NewtonsoftJson.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
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

            Car carFromRepo = _unit.CarRepository.Get(x => x.Id == id && (bool)x.IsApproved,
                includer: x => x.Include(car => car.ManagedByNavigation)
                                .Include(car => car.AssignedCar).ThenInclude(assign => assign.Account)
                                .Include(c => c.Issue).ThenInclude(issue => issue.Type));
            if (carFromRepo == null)
            {
                throw new NotFoundException("Car not found");
            }
            var actorId = int.Parse(claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

            if (role.Equals(Roles.Manager))
            {
                if (carFromRepo.ManagedBy != actorId)
                {
                    throw new PermissionDeniedException("Permission denied");
                }
            }

            if (role.Equals(Roles.Staff))
            {
                var assigned = carFromRepo.AssignedCar.FirstOrDefault(x => (bool)x.IsAvailable);

                if (assigned == null || assigned.AccountId != actorId)
                {
                    throw new PermissionDeniedException("Permission denied");
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

            dto = _unit.CarRepository.GetAll(page, limit, x => x.IsApproved == filter.IsApproved,
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

            if (filter.IsApproved.GetValueOrDefault())
            {
                dto.Result = dto.Result.Where(car => car.Name.Contains(searchValue));
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

        public void SetManagedBy(CarManagedByUpdateDto dto)
        {
            var car = _unit.CarRepository.Get(x => x.Id == dto.CarId && (bool)x.IsApproved && (bool)x.IsAvailable, x => x.AssignedCar);

            if (car == null)
            {
                throw new NotFoundException("Car not found");
            }

            if (car.ManagedBy != null && car.ManagedBy != dto.ManagerId)
            {
                var assignedList = car.AssignedCar.Where(x => x.IsAvailable == true);

                foreach (var assign in assignedList)
                {
                    assign.IsAvailable = false;
                    assign.RemoveAt = DateTime.UtcNow.AddHours(7);
                }
            }

            car.ManagedBy = dto.ManagerId;
            _unit.CarRepository.Update(car);
            _unit.SaveChanges();
        }

        public void SetActivation(int id, CarActivationDto dto)
        {
            var car = _unit.CarRepository.Get(x => x.Id == id && (bool)x.IsApproved);

            if (car == null)
            {
                throw new NotFoundException("Car not found");
            }

            if (dto.IsAvailable != car.IsAvailable)
            {
                if (!dto.IsAvailable)
                {
                    var assignedList = car.AssignedCar.Where(x => x.IsAvailable == true);

                    foreach (var assign in assignedList)
                    {
                        assign.IsAvailable = false;
                        assign.RemoveAt = DateTime.UtcNow.AddHours(7);
                    }
                }

                car.IsAvailable = dto.IsAvailable;
                _unit.CarRepository.Update(car);
                _unit.SaveChanges();
            }
        }

        public void CreateNewCarByDevice(string deviceId)
        {
            var defaultConfgiUrl = _unit.DefaultConfigurationRepository.GetAll().FirstOrDefault().ConfigUrl;
            if (defaultConfgiUrl.IsNullOrEmpty())
            {
                throw new NotFoundException("Default Config Not Inited");
            }
            var configurl = String.Empty;
            var webRequest = WebRequest.Create(defaultConfgiUrl);

            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                string contentType = response.ContentType;
                var strContent = reader.ReadToEnd();
                byte[] byteArray = Encoding.ASCII.GetBytes(strContent);
                MemoryStream stream = new MemoryStream(byteArray);
                string fileExtension = contentType[(contentType.IndexOf('/') + 1)..];
                configurl = FirebaseService.UploadFileToFirebaseStorage(stream, ("CarConfig" + deviceId).GetHashString() + "." + fileExtension, "CarConfig", _config).Result;
            }

            Car car = new Car { DeviceId = deviceId, ConfigUrl = configurl };
            _unit.CarRepository.Add(car);
            _unit.SaveChanges();
        }

        public void RegisterNewCar(int id, CarApprovalDto formDto)
        {
            var carFromRepo = _unit.CarRepository.Get(car => car.Id == id);

            if (carFromRepo == null || carFromRepo.IsApproved.GetValueOrDefault())
            {
                throw new NotFoundException("Car not found");
            }

            if (formDto.IsApproved)
            {
                CarApprovalDto carToPatch = _mapper.Map<CarApprovalDto>(carFromRepo);

                CarApprovalDto dto = new CarApprovalDto { ManagedBy = formDto.ManagedBy, Name = formDto.Name, IsApproved = formDto.IsApproved };
                JsonMergePatchDocument<CarApprovalDto> patchDto = PatchBuilder.Build(carToPatch, dto);
                patchDto.ApplyTo(carToPatch);

                _mapper.Map(carToPatch, carFromRepo);

                if (formDto.ConfigFile != null)
                {
                    carFromRepo.ConfigUrl = FirebaseService.UploadFileToFirebaseStorage(formDto.ConfigFile, ("CarConfig" + carFromRepo.DeviceId).GetHashString(), "CarConfig", _config).Result;
                }

                if (formDto.ImageFile != null)
                {
                    carFromRepo.Image = FirebaseService.UploadFileToFirebaseStorage(formDto.ConfigFile, ("CarImage" + carFromRepo.DeviceId).GetHashString(), "CarConfig", _config).Result;
                }
            }
            else
            {
                carFromRepo.IsApproved = false;
            }
            _unit.SaveChanges();

        }

    }
}
