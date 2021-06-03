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
            Issue = new HashSet<Issue>();
        }

        public int Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public int? SoftVersion { get; set; }
        public bool? IsConnecting { get; set; }
        public bool? IsAvailable { get; set; }
        public int? ModelId { get; set; }
        public int? ConfigId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string DeviceId { get; set; }
        public bool IsApproved { get; set; }
        public int? CreatedBy { get; set; }

        public virtual Configuration Config { get; set; }
        public virtual Account CreatedByNavigation { get; set; }
        public virtual ModelVersion Model { get; set; }
        public virtual SoftwareVersion SoftVersionNavigation { get; set; }
        public virtual ICollection<AssignedCar> AssignedCar { get; set; }
        public virtual ICollection<Issue> Issue { get; set; }
    }
}
