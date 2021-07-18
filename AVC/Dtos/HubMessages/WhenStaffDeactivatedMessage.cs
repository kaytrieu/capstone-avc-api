using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.HubMessages
{
    public class WhenStaffDeactivatedMessage
    {
        public int ReceiverId { get; set; }
        public int StaffId { get; set; }
        public string Message { get; set; }

        public WhenStaffDeactivatedMessage(int receiverId, int deactivatedId, string message)
        {
            ReceiverId = receiverId;
            StaffId = deactivatedId;
            Message = message;
        }
    }
}
