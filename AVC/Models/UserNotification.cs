using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AVC.Models
{
    public partial class UserNotification
    {
        public UserNotification(int? receiverId, string message, string type)
        {
            ReceiverId = receiverId;
            Message = message;
            Type = type;
        }

        public UserNotification()
        {
        }

        public int Id { get; set; }
        public int? ReceiverId { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }

        public virtual Account Receiver { get; set; }
    }
}
