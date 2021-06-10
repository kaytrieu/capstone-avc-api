using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AVC.Models
{
    public partial class Car
    {
        public Car()
        {
            AssignedCar = new HashSet<AssignedCar>();
            CarConfig = new HashSet<CarConfig>();
            Issue = new HashSet<Issue>();
        }

        public int Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public bool? IsConnecting { get; set; }
        public bool? IsAvailable { get; set; }
        public int? ModelId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string DeviceId { get; set; }
        public bool IsApproved { get; set; }
        public int? ManagedBy { get; set; }

        public virtual Account ManagedByNavigation { get; set; }
        public virtual ModelVersion Model { get; set; }
        public virtual ICollection<AssignedCar> AssignedCar { get; set; }
        public virtual ICollection<CarConfig> CarConfig { get; set; }
        public virtual ICollection<Issue> Issue { get; set; }
    }
}
