using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.ProfileDtos
{
    public class ProfileUpdateDto
    {
        [MaxLength(9)][MinLength(9)]
        [Phone]
        public string Phone { get; set; }
        public IFormFile AvatarImage { get; set; }
    }
}
