using AVC.Constant;
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

        [EmailAddress]
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
        [Phone]
        public string Phone { get; set; }
        [Url]
        public string Avatar { get; set; }
        public int? ManagedBy { get; set; }
    }
}
