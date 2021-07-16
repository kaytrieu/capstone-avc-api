using AVC.Dtos.UserNotificationDtos;
using AVC.Dtos.PagingDtos;
using AVC.Dtos.QueryFilter;
using AVC.Models;
using Microsoft.AspNetCore.Http;
using Morcatko.AspNetCore.JsonMergePatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Services.Interfaces
{
    public  interface IUserNotificationService
    {
        PagingResponseDto<UserNotificationReadDto> GetUserNotificationList(UserNotificationQueryFilter filter);
        int GetUserNotificationCount(int accountId);
    }
}
