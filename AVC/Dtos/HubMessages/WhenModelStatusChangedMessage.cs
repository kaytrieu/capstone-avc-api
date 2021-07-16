using AVC.Dtos.ModelDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.HubMessages
{
    public class WhenModelStatusChangedMessage
    {
        public int ReceiverId { get; set; }
        public string Message { get; set; }
        public int ModelId { get; set; }

        public WhenModelStatusChangedMessage(int receiverId, int modelId, string message)
        {
            ReceiverId = receiverId;
            Message = message;
            ModelId = modelId;
        }
    }
}
