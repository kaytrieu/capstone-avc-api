using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.HubMessages
{
    public class WhenManagerChangeAssignedCarMessage
    {
        public int ReceiverId { get; set; }
        public int CarId { get; set; }
        public string Message { get; set; }

        public WhenManagerChangeAssignedCarMessage(int receiverId, int carId, string message)
        {
            ReceiverId = receiverId;
            CarId = carId;
            Message = message;
        }
    }
}
