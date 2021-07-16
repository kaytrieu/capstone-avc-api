using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.HubMessages
{
    public class WhenIssueCreatedMessage
    {
        public List<int> ReceiverIdList { get; set; }
        public int CarId { get; set; }
        public string Message { get; set; }

        public WhenIssueCreatedMessage(List<int> receiverIdList, int carId, string message)
        {
            ReceiverIdList = receiverIdList;
            CarId = carId;
            Message = message;
        }
    }
}
