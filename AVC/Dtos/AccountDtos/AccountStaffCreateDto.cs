using AVC.Constant;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AVC.Dtos.AccountDtos
{
    public class AccountStaffCreateDto
    {
        public AccountStaffCreateDto()
        {
            Salt = BCrypt.Net.BCrypt.GenerateSalt();
            RoleId = Roles.StaffId;
            Password = BCrypt.Net.BCrypt.HashPassword("123123", Salt);
        }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }
        public string Password { get;}
        public string Salt { get; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [JsonIgnore]
        public int RoleId { get; }
        [Phone]
        [MaxLength(9)][MinLength(9)]
        public string Phone { get; set; }
        public int? ManagedBy { get; set; }
    }
}
