using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.HubMessages
{
    public class WhenManagerDeactivatedMessage
    {
        public List<int> ReceiverIdList { get; set; }
        public int DeactivatedId { get; set; }
        public string Message { get; set; }

        public WhenManagerDeactivatedMessage(List<int> receiverIdList, int deactivatedId, string message)
        {
            ReceiverIdList = receiverIdList;
            DeactivatedId = deactivatedId;
            Message = message;
        }
    }
}
