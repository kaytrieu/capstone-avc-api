using AVC.Dtos.AuthenticationDtos;
using AVC.Dtos.ReponseDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Services.Interfaces
{
    public interface IAuthenticateService
    {
        AuthenticationReadDto CheckLogin(AuthenticationPostDto dto);
        void SendResetPasswordEmail(string email);
        void SetNewPassword(NewPasswordDto dto);
    }
}
