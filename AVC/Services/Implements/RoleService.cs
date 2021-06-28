using AutoMapper;
using AVC.Dtos.RoleDtos;
using AVC.Repositories.Interface;
using AVC.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Services.Implements
{
    public class RoleService : BaseService, IRoleService
    {
        public RoleService(IUnitOfWork unit, IMapper mapper, IConfiguration config, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor) : base(unit, mapper, config, urlHelper, httpContextAccessor)
        {
        }

        public IEnumerable<RoleReadDto> GetRoleList()
        {
            var roleList = _unit.RoleRepository.GetAll(x => x.IsAvailable == true && !x.Name.Equals("Admin"));
            var response = _mapper.Map<IEnumerable<RoleReadDto>>(roleList);
            return response;
        }
    }
}
