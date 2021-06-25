using AutoMapper;
using AVC.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Services.Implements
{
    public class BaseService
    {
        public readonly IUnitOfWork _unit;
        public readonly IMapper _mapper;
        public readonly IConfiguration _config;
        public readonly IUrlHelper _urlHelper;
        public IHttpContextAccessor _httpContextAccessor;

        public BaseService(IUnitOfWork unit, IMapper mapper, IConfiguration config, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor)
        {
            _unit = unit;
            _mapper = mapper;
            _config = config;
            _urlHelper = urlHelper;
            _httpContextAccessor = httpContextAccessor;

        }
    }
}
