using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AVC.Models
{
    public partial class Issue
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public int CarId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public bool? IsAvailable { get; set; }
        public string Location { get; set; }

        public virtual Car Car { get; set; }
        public virtual IssueType Type { get; set; }
    }
}
