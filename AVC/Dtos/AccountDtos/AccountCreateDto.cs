using System.ComponentModel.DataAnnotations;

namespace AVC.Dtos.AccountDtos
{
    public class AccountCreateDto
    {
        public AccountCreateDto()
        {
            Salt = BCrypt.Net.BCrypt.GenerateSalt();
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
        public int RoleId { get; set; }
        [Phone]
        public string Phone { get; set; }
        [Url]
        public string Avatar { get; set; }
        public string Address { get; set; }
        [Required]
        public int? GenderId { get; set; }
        public int? CreatedBy { get; private set; }

        public void SetCreatedById(int createdId)
        {
            CreatedBy = createdId;
        }

    }
}
