using AVC.Dtos.AccountDtos;
using AVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.CarDtos
{
    public class CarListReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsConnecting { get; set; }
        public bool? IsAvailable { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsRunning { get; set; }
        public string DeviceId { get; set; }
        public bool? IsApproved { get; set; }
        public string Image { get; set; }
        public virtual AccountNotManagedByReadDto ManagedBy { get; set; }
        public virtual AccountStaffAssignToReadDto AssignTo { get; set; }

    }
}
