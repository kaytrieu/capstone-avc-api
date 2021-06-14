using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.ProfileDtos
{
    public class ProfileUpdateDto
    {
        public string Phone { get; set; }
        public IFormFile AvatarImage { get; set; }
    }
}
