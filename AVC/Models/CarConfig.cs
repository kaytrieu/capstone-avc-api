using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AVC.Models
{
    public partial class CarConfig
    {
        public int Id { get; set; }
        public int? CarId { get; set; }
        public DateTime? ConfigAt { get; set; }
        public DateTime? RemoveAt { get; set; }
        public bool? IsAvailable { get; set; }
        public int? ConfigId { get; set; }
        public int ConfigBy { get; set; }

        public virtual Car Car { get; set; }
        public virtual Configuration Config { get; set; }
        public virtual Account ConfigByNavigation { get; set; }
    }
}
