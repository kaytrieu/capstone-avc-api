using AutoMapper;
using AVC.Constant;
using AVC.Dtos.UserNotificationDtos;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using AVC.Hubs;

namespace AVC.Services.Implements
{
    public class UserNotificationService : BaseService, IUserNotificationService
    {
        public UserNotificationService(IUnitOfWork unit, IMapper mapper, IConfiguration config, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor, IHubContext<AVCHub> hubContext) : base(unit, mapper, config, urlHelper, httpContextAccessor, hubContext)
        {
        }

        public int GetUserNotificationCount(int accountId)
        {
            var count = _unit.UserNotificationRepository.GetAll(x => x.ReceiverId == accountId && !x.IsRead).Count();

            return count;
        }

        public PagingResponseDto<UserNotificationReadDto> GetUserNotificationList(UserNotificationQueryFilter filter)
        {
            var notiList = _unit.UserNotificationRepository.GetAll(filter.Page, filter.Limit, x => x.ReceiverId == filter.ReceiverId);
            var dtoList = _mapper.Map<IEnumerable<UserNotificationReadDto>>(notiList.Result);

            var response = new PagingResponseDto<UserNotificationReadDto>(dtoList, filter.Page, filter.Limit);

            foreach (var noti in response.Result)
            {
                notiList.Result.Where(x => x.Id == noti.Id).FirstOrDefault().IsRead = true;
            }

            _unit.SaveChanges();

            if (filter.Limit > 0)
            {
                if ((double)notiList.Count / filter.Limit > filter.Page)
                {
                    response.NextPage = _urlHelper.Link(null, new { page = filter.Page + 1, filter.Limit, filter.ReceiverId });
                }

                if (filter.Page > 1)
                    response.PreviousPage = _urlHelper.Link(null, new { page = filter.Page - 1, filter.Limit, filter.ReceiverId });
            }

            return response;
        }

    
    }
}
