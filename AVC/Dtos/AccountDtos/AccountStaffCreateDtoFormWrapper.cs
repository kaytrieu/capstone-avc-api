using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.AccountDtos
{
    public class AccountStaffCreateDtoFormWrapper
    {
        public IFormFile AvatarImage { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [MaxLength(9)][MinLength(9)]
        [Phone]
        public string Phone { get; set; }
        public int? ManagedBy { get; set; }
    }
}
