using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.AuthenticationDtos
{
    public class NewPasswordDto
    {
        public NewPasswordDto()
        {
            Salt = BCrypt.Net.BCrypt.GenerateSalt();
        }
        [Required]
        public string Email { get; set; }
        private string password;
        public string Password
        {
            get { return password; }
            set { password = BCrypt.Net.BCrypt.HashPassword(value, Salt); }
        }
        [Required]
        public string SecurityKey { get; set; }
        public string Salt { get; }
    }
}
