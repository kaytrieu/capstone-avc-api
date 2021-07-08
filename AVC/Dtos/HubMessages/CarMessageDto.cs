using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.HubMessages
{
    public class CarMessageDto
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string ConfigUrl { get; set; }
        public int ModelId { get; set; }
        public string ModelURL { get; set; }
    }
}
