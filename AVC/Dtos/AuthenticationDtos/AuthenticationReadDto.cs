//using AVC.Dtos.AccountDtos;

using AVC.Dtos.AccountDtos;
using AVC.Dtos.ProfileDtos;

namespace AVC.Dtos.AuthenticationDtos
{
    public class AuthenticationReadDto
    {
        public string Token { get; set; }
        public AccountReadDto Account { get; set; }
    }
}
