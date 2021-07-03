using AutoMapper;
using AVC.Constant;
using AVC.Dtos.CarDtos;
using AVC.Dtos.PagingDtos;
using AVC.Dtos.QueryFilter;
using AVC.Models;
using AVC.Repositories.Interface;
using AVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Morcatko.AspNetCore.JsonMergePatch;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static AVC.Extensions.Extensions.Extensions;

namespace AVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ILogger<CarsController> _logger;

        public CarsController(ICarService carService, IMapper mapper, IConfiguration config, ILogger<CarsController> logger)
        {
            _carService = carService;
            _mapper = mapper;
            _config = config;
            _logger = logger;
        }


        // GET: api/Cars
        [HttpGet]
        public ActionResult<PagingResponseDto<CarListReadDto>> GetCarList([FromQuery] CarQueryFilter filter)
        {
            var response = _carService.GetCarList(filter);

            return Ok(response);
        }

        // GET: api/Cars/5
        [HttpGet("{id}")]
        public ActionResult<CarReadDto> GetCar(int id)
        {
            var respone = _carService.GetCarDetail(id);

            return Ok(respone);
        }

        [AuthorizeRoles(Roles.Admin)]
        [HttpPut("managedBy")]
        public ActionResult SetManagedBy(CarManagedByUpdateDto dto)
        {
            _carService.SetManagedBy(dto);
            return Ok();
        }

        [AuthorizeRoles(Roles.Admin)]
        [HttpPut("{id}/activation")]
        public ActionResult SetActivation(int id, CarActivationDto dto)
        {
            _carService.SetActivation(id, dto);
            return Ok();
        }

        [HttpPost]
        public ActionResult CreateCarByDeviceId(string deviceId)
        {
            _carService.CreateNewCarByDevice(deviceId);
            return Ok();
        }

        [HttpPut("{id}/Approvement")]
        public ActionResult RegisterCar(int id, [FromForm] CarApprovalDto dto)
        {
            _carService.RegisterNewCar(id, dto);
            return Ok();
        }

        // PUT: api/Cars/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutCar(int id, Car car)
        //{
        //    return Ok();

        //}

        //// POST: api/Cars
        //[HttpPost]
        //public async Task<ActionResult<Car>> PostCar(Car car)
        //{

        //    return Ok();
        //}
    }
}
