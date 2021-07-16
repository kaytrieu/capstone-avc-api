using AutoMapper;
using AVC.Dtos.UserNotificationDtos;
using AVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Profiles
{
    public class UserNotificationProfile : Profile
    {
        public UserNotificationProfile()
        {
            CreateMap<UserNotification, UserNotificationReadDto>();
        }
    }
}
