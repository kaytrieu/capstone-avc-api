using AutoMapper;
using AVC.Constant;
using AVC.Dtos.CarDtos;
using AVC.Dtos.HubMessages;
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


            var cars = _mapper.Map<IEnumerable<CarListReadDto>>(dto.Result);

            var response = new PagingResponseDto<CarListReadDto>(cars, page, limit);

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
            var car = _unit.CarRepository.Get(x => x.Id == id && (bool)x.IsApproved, car => car.AssignedCar);

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

        public DefaultCarConfigDto GetDefaultCarConfig()
        {
            return _mapper.Map<DefaultCarConfigDto>(_unit.DefaultConfigurationRepository.GetAll().FirstOrDefault());
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

                if (formDto.ImageFile != null)
                {
                    carFromRepo.Image = FirebaseService.UploadFileToFirebaseStorage(formDto.ImageFile, ("CarImage" + carFromRepo.DeviceId).GetHashString(), "CarImage", _config).Result;
                }
            }
            else
            {
                carFromRepo.IsApproved = false;
            }
            _unit.SaveChanges();

        }

        public void Update(int id, CarUpdateDto dto)
        {
            var carFromRepo = _unit.CarRepository.Get(car => car.Id == id);

            if (carFromRepo == null || !carFromRepo.IsApproved.GetValueOrDefault())
            {
                throw new NotFoundException("Car not found");
            }

            carFromRepo.Name = dto.Name;

            _unit.SaveChanges();

        }

        public void UpdateImage(int id, IFormFile image)
        {
            var carFromRepo = _unit.CarRepository.Get(car => car.Id == id);

            if (carFromRepo == null || !carFromRepo.IsApproved.GetValueOrDefault())
            {
                throw new NotFoundException("Car not found");
            }

            if (image != null)
            {
                carFromRepo.Image = FirebaseService.UploadFileToFirebaseStorage(image, ("CarImage" + carFromRepo.DeviceId).GetHashString(), "CarImage", _config).Result;
            }

            _unit.SaveChanges();
        }

        public void UpdateConfig(int id, IFormFile config)
        {
            var carFromRepo = _unit.CarRepository.Get(car => car.Id == id);

            if (carFromRepo == null || !carFromRepo.IsApproved.GetValueOrDefault())
            {
                throw new NotFoundException("Car not found");
            }

            if (config != null)
            {
                carFromRepo.ConfigUrl = FirebaseService.UploadFileToFirebaseStorage(config, ("CarConfig" + carFromRepo.DeviceId).GetHashString(), "CarConfig", _config).Result;
            }

            _unit.SaveChanges();
        }
        public void UpdateDefaultConfig(IFormFile config)
        {
            var defaultConfig = _unit.DefaultConfigurationRepository.GetAll().FirstOrDefault();

            if (config != null)
            {
                defaultConfig.ConfigUrl = FirebaseService.UploadFileToFirebaseStorage(config, "DefaultConfig", "CarConfig", _config).Result;
                defaultConfig.LastModified = DateTime.UtcNow.AddHours(7);
                _unit.SaveChanges();

            }

        }

        public void AssignCar(int carId, int? staffId)
        {
            var carFromRepo = _unit.CarRepository.Get(car => car.Id == carId, x => x.AssignedCar);

            if (carFromRepo == null || !carFromRepo.IsApproved.GetValueOrDefault())
            {
                throw new NotFoundException("Car not found");
            }

            var asCarFromRepo = carFromRepo.AssignedCar.Where(x => x.IsAvailable == true).FirstOrDefault();

            if (asCarFromRepo != null)
            {
                if (asCarFromRepo.AccountId == staffId)
                {
                    return;
                }
                asCarFromRepo.IsAvailable = false;
                asCarFromRepo.RemoveAt = DateTime.UtcNow.AddHours(7);
            }

            if (staffId != null)
            {
                var staffFromRepo = _unit.AccountRepository.Get(x => x.Id == staffId && x.ManagedBy == carFromRepo.ManagedBy && (bool)x.IsAvailable);

                if (staffFromRepo == null)
                {
                    throw new NotFoundException("Staff not found");
                }

                var claims = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).Claims;

                var actorId = int.Parse(claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

                AssignedCar asCar = new AssignedCar { AccountId = (int)staffId, CarId = carId, AssignedBy = actorId };

                _unit.AssignedCarRepository.Add(asCar);
            }

            _unit.SaveChanges();


            //var asCarDto = _mapper.Map<AssignedCarInCarDetailReadDto>(asCar);

            //return asCarDto;
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

        public HandleCarConnectedObject HandleCarConnected(string deviceId)
        {
            var carFromRepo = _unit.CarRepository.Get(car => car.DeviceId == deviceId, car => car.AssignedCar);
            List<int> accountIdList = new List<int>();
            HandleCarConnectedObject message = null;
            CarConnectedMessage carConnectedMessage = null;
            CarMessageDto carMessageDto = null;

            if (carFromRepo == null)
            {
                CreateNewCarByDevice(deviceId);
            }
            else
            {
                if (carFromRepo.IsApproved.GetValueOrDefault() && carFromRepo.IsAvailable.GetValueOrDefault())
                {
                    carFromRepo.IsConnecting = true;
                    _unit.SaveChanges();


                    int adminId = _unit.AccountRepository.Get(acc => acc.RoleId == Roles.AdminId).Id;
                    accountIdList.Add(adminId);

                    if (carFromRepo.ManagedBy != null)
                    {
                        accountIdList.Add((int)carFromRepo.ManagedBy);
                    }

                    var assigned = carFromRepo.AssignedCar.Where(x => x.IsAvailable == true).FirstOrDefault();

                    if (assigned != null)
                    {
                        accountIdList.Add(assigned.AccountId);
                    }

                    carConnectedMessage = new CarConnectedMessage(accountIdList, carFromRepo.Id);
                    carMessageDto = _mapper.Map<CarMessageDto>(carFromRepo);
                }
            }

            message = new HandleCarConnectedObject(carConnectedMessage, carMessageDto);

            return (message);

        }

        public Car GetCarModel(int carId)
        {
            var carFromRepo = _unit.CarRepository.Get(car => car.Id == carId);

            if (carFromRepo != null)
            {
                return carFromRepo;
            }

            return null;
        }

        public WhenCarRunningMessage HandleWhenCarRunning(string deviceId)
        {
            var carFromRepo = _unit.CarRepository.Get(car => car.DeviceId == deviceId, car => car.AssignedCar);

            if (carFromRepo != null)
            {
                carFromRepo.IsRunning = true;
                _unit.SaveChanges();

                List<int> relatedAccount = new List<int>();

                int adminId = _unit.AccountRepository.Get(acc => acc.RoleId == Roles.AdminId).Id;
                relatedAccount.Add(adminId);

                if (carFromRepo.ManagedBy != null)
                {
                    relatedAccount.Add((int)carFromRepo.ManagedBy);
                }

                var assigned = carFromRepo.AssignedCar.Where(x => x.IsAvailable == true).FirstOrDefault();

                if (assigned != null)
                {
                    relatedAccount.Add(assigned.AccountId);
                }

                WhenCarRunningMessage whenCarRunningMessage = new WhenCarRunningMessage(relatedAccount, carFromRepo.Id);

                return whenCarRunningMessage;
            }

            return null;
        }

        public WhenCarStoppingMessage HandleWhenCarStopping(string deviceId)
        {
            var carFromRepo = _unit.CarRepository.Get(car => car.DeviceId == deviceId, car => car.AssignedCar);

            if (carFromRepo != null)
            {
                carFromRepo.IsRunning = false;
                _unit.SaveChanges();

                List<int> relatedAccount = new List<int>();

                int adminId = _unit.AccountRepository.Get(acc => acc.RoleId == Roles.AdminId).Id;

                relatedAccount.Add(adminId);

                if (carFromRepo.ManagedBy != null)
                {
                    relatedAccount.Add((int)carFromRepo.ManagedBy);
                }

                var assigned = carFromRepo.AssignedCar.Where(x => x.IsAvailable == true).FirstOrDefault();

                if (assigned != null)
                {
                    relatedAccount.Add(assigned.AccountId);
                }

                WhenCarStoppingMessage whenCarStoppingMessage = new WhenCarStoppingMessage(relatedAccount, carFromRepo.Id);

                return whenCarStoppingMessage;
            }

            return null;
        }

        public WhenCarDisconnectedMessage HandleWhenCarDisconnected(int carId)
        {
            var carFromRepo = _unit.CarRepository.Get(car => car.Id == carId, car => car.AssignedCar);

            if (carFromRepo != null)
            {
                carFromRepo.IsRunning = false;
                carFromRepo.IsConnecting = false;
                _unit.SaveChanges();

                List<int> relatedAccount = new List<int>();

                int adminId = _unit.AccountRepository.Get(acc => acc.RoleId == Roles.AdminId).Id;

                relatedAccount.Add(adminId);

                if (carFromRepo.ManagedBy != null)
                {
                    relatedAccount.Add((int)carFromRepo.ManagedBy);
                }

                var assigned = carFromRepo.AssignedCar.Where(x => x.IsAvailable == true).FirstOrDefault();

                if (assigned != null)
                {
                    relatedAccount.Add(assigned.AccountId);
                }

                WhenCarDisconnectedMessage whenCarDisconnectedMessage = new WhenCarDisconnectedMessage(relatedAccount, carFromRepo.Id);

                return whenCarDisconnectedMessage;
            }

            return null;
        }

    }
}
