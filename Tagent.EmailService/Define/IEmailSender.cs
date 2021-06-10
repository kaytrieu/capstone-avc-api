using System;
using System.Collections.Generic;
using System.Text;

namespace Tagent.EmailService.Define
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
    }
}
