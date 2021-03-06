using AVC.Dtos.AccountDtos;
using AVC.Dtos.IssueDtos;
using AVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.CarDtos
{
    public class CarAssignedReadDto
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public bool IsConnecting { get; set; }
        public bool? IsAvailable { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string DeviceId { get; set; }
        public string ConfigUrl { get; set; }
        public bool? IsApproved { get; set; }
        public bool IsRunning { get; set; }

    }
}
