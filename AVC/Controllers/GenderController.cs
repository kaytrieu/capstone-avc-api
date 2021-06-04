using AutoMapper;
using AVC.Dtos.GenderDtos;
using AVC.GenericRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GendersController : ControllerBase
    {
        private readonly IGenderRepository _repository;
        private readonly IMapper _mapper;


        public GendersController(IGenderRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/Genders
        /// <summary>
        /// Get all genders of system
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<GenderReadDto>> GetGender()
        {
            var GenderList = _repository.GetAll(x => x.IsAvailable == true);
            var response = _mapper.Map<IEnumerable<GenderReadDto>>(GenderList);
            return Ok(new {Result = response });
        }

    }
}
