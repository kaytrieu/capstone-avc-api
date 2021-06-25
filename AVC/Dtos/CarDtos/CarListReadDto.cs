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
        public string Image { get; set; }
        public string Name { get; set; }
        public bool? IsAvailable { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string DeviceId { get; set; }
        public bool IsApproved { get; set; }
        public int? ManagedBy { get; set; }
        public virtual AccountManagerReadDto ManagedByNavigation { get; set; }
        public virtual int AssignedToId { get; set; }
        public virtual string AssignedToEmail { get; set; }

    }
}
