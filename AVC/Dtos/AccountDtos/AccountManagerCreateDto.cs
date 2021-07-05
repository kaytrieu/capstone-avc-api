using AVC.Constant;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AVC.Dtos.AccountDtos
{
    public class AccountManagerCreateDto
    {
        public AccountManagerCreateDto()
        {
            Salt = BCrypt.Net.BCrypt.GenerateSalt();
            RoleId = Roles.ManagerId;
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
        [Required]
        public int RoleId { get; }
        [MaxLength(11)]
        [Phone]
        public string Phone { get; set; }
    }
}
