using AutoMapper;
using AVC.Hubs;
using AVC.Models;
using AVC.Repositories.Interface;
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
    public class BaseService
    {
        public readonly IUnitOfWork _unit;
        public readonly IMapper _mapper;
        public readonly IConfiguration _config;
        public readonly IUrlHelper _urlHelper;
        public IHttpContextAccessor _httpContextAccessor;
        private IUnitOfWork unit;
        private IMapper mapper;
        private IConfiguration config;
        private IUrlHelper urlHelper;
        private IHttpContextAccessor httpContextAccessor;
        public readonly IHubContext<AVCHub> _hubContext;



        public BaseService(IUnitOfWork unit, IMapper mapper, IConfiguration config, 
                            IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor, IHubContext<AVCHub> hubContext)
        {
            _unit = unit;
            _mapper = mapper;
            _config = config;
            _urlHelper = urlHelper;
            _hubContext = hubContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public BaseService(IUnitOfWork unit, IMapper mapper, IConfiguration config, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor)
        {
            this.unit = unit;
            this.mapper = mapper;
            this.config = config;
            this.urlHelper = urlHelper;
            this.httpContextAccessor = httpContextAccessor;
        }

        internal void AddNewNotification(int receiverId, string message, string type, bool saveChange = false)
        {
            var noti = new UserNotification(receiverId, message, type);
            _unit.UserNotificationRepository.Add(noti);

            if (saveChange)
            {
                _unit.SaveChanges();
            }
        }
    }
}
