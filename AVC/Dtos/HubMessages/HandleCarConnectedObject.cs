using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.HubMessages
{
    public class HandleCarConnectedObject
    {
        public CarConnectedMessage carConnectedMessage { get; set; }
        public CarMessageDto carMessageDto { get; set; }

        public HandleCarConnectedObject(CarConnectedMessage carConnectedMessage, CarMessageDto carMessageDto)
        {
            this.carConnectedMessage = carConnectedMessage;
            this.carMessageDto = carMessageDto;
        }
    }
}
