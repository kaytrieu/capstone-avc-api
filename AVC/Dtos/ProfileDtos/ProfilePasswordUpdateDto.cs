using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.ProfileDtos
{
    public class ProfilePasswordUpdateDto
    {
        public ProfilePasswordUpdateDto()
        {
            salt = BCrypt.Net.BCrypt.GenerateSalt();
        }

        private string newPassword;
        public string NewPassword
        {
            get { return newPassword; }
            set { newPassword = BCrypt.Net.BCrypt.HashPassword(value, salt); }
        }
        public string OldPassword { get; set; }
        private string salt;
    }
}
