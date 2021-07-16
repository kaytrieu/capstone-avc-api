using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.HubMessages
{
    public class WhenAdminChangeCarManagedByMessage
    {
        public int ReceiverId { get; set; }
        public int CarId { get; set; }
        public string Message { get; set; }

        public WhenAdminChangeCarManagedByMessage(int managerId, int carId, string message)
        {
            this.ReceiverId = managerId;
            this.CarId = carId;
            this.Message = message;
        }

        public WhenAdminChangeCarManagedByMessage()
        {
        }
    }
}
