using AVC.Dtos.RoleDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Services.Interfaces
{
    public  interface IRoleService
    {
        IEnumerable<RoleReadDto> GetRoleList();
    }
}
