//using AVC.Dtos.AccountDtos;

using AVC.Dtos.AccountDtos;

namespace AVC.Dtos.AuthenticationDtos
{
    public class AuthenticationReadDto
    {
        public string Token { get; set; }
        public AccountReadAfterAuthenDto Account { get; set; }
    }
}
