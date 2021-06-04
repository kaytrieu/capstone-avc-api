using AutoMapper;
using AVC.Dtos.RoleDtos;
using AVC.GenericRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleRepository _repository;
        private readonly IMapper _mapper;


        public RolesController(IRoleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/Roles
        /// <summary>
        /// Get all Role of System
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<RoleReadDto>> GetRole()
        {
            var roleList = _repository.GetAll(x => x.IsAvailable == true);
            var response = _mapper.Map<IEnumerable<RoleReadDto>>(roleList);
            return Ok(new {Result = response });
        }

    }
}
