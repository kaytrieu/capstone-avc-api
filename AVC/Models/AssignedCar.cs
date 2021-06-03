using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AVC.Models
{
    public partial class AssignedCar
    {
        public int CarId { get; set; }
        public bool? IsAvailable { get; set; }
        public DateTime? AssignedAt { get; set; }
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? AssignedBy { get; set; }

        public virtual Account Account { get; set; }
        public virtual Account AssignedByNavigation { get; set; }
        public virtual Car Car { get; set; }
    }
}
