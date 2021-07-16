using AutoMapper;
using AVC.Dtos.QueryFilter;
using AVC.Dtos.UserNotificationDtos;
using AVC.Repositories.Interface;
using AVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserNotificationsController : ControllerBase
    {
        private readonly IUserNotificationService _userNotificationService;


        public UserNotificationsController(IUserNotificationService userNotificationService)
        {
            _userNotificationService = userNotificationService;
        }

        // GET: api/UserNotifications
        /// <summary>
        /// Get all UserNotification of System
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<UserNotificationReadDto>> GetUserNotification([FromQuery] UserNotificationQueryFilter filter)
        {
            var dto = _userNotificationService.GetUserNotificationList(filter);
            return Ok(new { Result = dto });
        }


        [HttpGet("{receiverId}/count")]
        public ActionResult<int> GetUserNotificationCount(int receiverId)
        {
            var dto = _userNotificationService.GetUserNotificationCount(receiverId);
            return Ok(new { Result = dto });
        }

    }
}
