using System.ComponentModel.DataAnnotations;

namespace AVC.Dtos.AuthenticationDtos
{
    public class AuthenticationPostDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
