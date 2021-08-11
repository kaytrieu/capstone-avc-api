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
            Password = BCrypt.Net.BCrypt.HashPassword("123123", Salt);
        }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }
        public string Password { get; }
        public string Salt { get; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public int RoleId { get; }
        [MaxLength(9)][MinLength(9)]
        [Phone]
        public string Phone { get; set; }
    }
}
