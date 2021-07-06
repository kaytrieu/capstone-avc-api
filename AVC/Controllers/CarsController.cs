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
        
        /// <summary>
        /// Get Default Config
        /// </summary>
        /// <returns>Config Dto</returns>
        [HttpGet("DefaultConfig")]
        public ActionResult<DefaultCarConfigDto> GetDefaultConfig()
        {
            var respone = _carService.GetDefaultCarConfig();

            return Ok(respone);
        }

        /// <summary>
        /// Edit ConfigFile
        /// </summary>
        /// <param name="configFile">Config File, null is no update</param>
        /// <returns></returns>
        [HttpPut("DefaultConfig")]
        public ActionResult UpdateConfig(IFormFile configFile)
        {
            _carService.UpdateDefaultConfig(configFile);
            return Ok();
        }

        /// <summary>
        /// Update Car ManagedBy
        /// </summary>
        /// <param name="dto">CarID, ManagerID, if ManagerId = null will delete managedBy</param>
        /// <returns>200</returns>
        [AuthorizeRoles(Roles.Admin)]
        [HttpPut("managedBy")]
        public ActionResult SetManagedBy(CarManagedByUpdateDto dto)
        {
            _carService.SetManagedBy(dto);
            return Ok();
        }

        /// <summary>
        /// Set Car Activation
        /// </summary>
        /// <param name="id">CarID</param>
        /// <param name="dto">True or false</param>
        /// <returns></returns>
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

        /// <summary>
        /// Approve or reject New Car
        /// </summary>
        /// <param name="id">CarID</param>
        /// <param name="dto">IsApproved = true to approve, false to reject</param>
        /// <returns></returns>
        [HttpPut("{id}/Approvement")]
        public ActionResult RegisterCar(int id, [FromForm] CarApprovalDto dto)
        {
            _carService.RegisterNewCar(id, dto);
            return Ok();
        }

        /// <summary>
        /// Update Car Name
        /// </summary>
        /// <param name="id">car Id</param>
        /// <param name="dto">Name of Car</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public ActionResult UpdateCar(int id, CarUpdateDto dto)
        {
            _carService.Update(id, dto);
            return Ok();
        }

        /// <summary>
        /// Assign Car for Staff
        /// </summary>
        /// <param name="id">CarId</param>
        /// <param name="staffId">null for remove all assigned</param>
        /// <returns></returns>
        [HttpPut("{id}/Assign")]
        public ActionResult AssignCar(int id, int? staffId)
        {
            _carService.AssignCar(id, staffId);
            return Ok();
        }

        /// <summary>
        /// Update Car Configuration
        /// </summary>
        /// <param name="id">Car Id</param>
        /// <param name="configFile">null to no update</param>
        /// <returns></returns>
        [HttpPut("{id}/Configuration")]
        public ActionResult UpdateConfig(int id, IFormFile configFile)
        {
            _carService.UpdateConfig(id, configFile);
            return Ok();
        }

        /// <summary>
        /// Update Car Image
        /// </summary>
        /// <param name="id">Car Id</param>
        /// <param name="imageFile">Null to no update</param>
        /// <returns></returns>
        [HttpPut("{id}/Image")]
        public ActionResult UpdateImage(int id, IFormFile imageFile)
        {
            _carService.UpdateImage(id, imageFile);
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
