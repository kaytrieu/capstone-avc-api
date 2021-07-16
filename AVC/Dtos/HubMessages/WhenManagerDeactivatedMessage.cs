using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.HubMessages
{
    public class WhenStaffDeactivatedMessage
    {
        public int ReceiverId { get; set; }
        public int DeactivatedId { get; set; }
        public string Message { get; set; }

        public WhenStaffDeactivatedMessage(int receiverId, int deactivatedId, string message)
        {
            ReceiverId = receiverId;
            DeactivatedId = deactivatedId;
            Message = message;
        }
    }
}
