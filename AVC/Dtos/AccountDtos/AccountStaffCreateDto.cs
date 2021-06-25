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
        }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }
        private string password;
        [Required]
        public string Password
        {
            get { return password; }
            set { password = BCrypt.Net.BCrypt.HashPassword(value, Salt); }
        }
        public string Salt { get; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [JsonIgnore]
        public int RoleId { get; }
        [Phone]
        public string Phone { get; set; }
        public int? ManagedBy { get; set; }
    }
}
