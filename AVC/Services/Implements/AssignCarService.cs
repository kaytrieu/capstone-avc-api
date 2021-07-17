using AutoMapper;
using AVC.Hubs;
using AVC.Repositories.Interface;
using AVC.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Services.Implements
{
    public class AssignCarService : BaseService, IAssignCarService
    {
        public AssignCarService(IUnitOfWork unit, IMapper mapper, IConfiguration config, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor, IHubContext<AVCHub> hubContext) : base(unit, mapper, config, urlHelper, httpContextAccessor, hubContext)
        {
        }
    }
}
