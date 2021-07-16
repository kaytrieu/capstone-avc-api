using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.HubMessages
{
    public class WhenAdminChangeStaffManagedByMessage
    {
        public int ReceiverId { get; set; }
        public int StaffId { get; set; }
        public string Message { get; set; }

    }
}
