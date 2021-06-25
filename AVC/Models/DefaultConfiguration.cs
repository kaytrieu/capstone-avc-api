using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AVC.Models
{
    public partial class DefaultConfiguration
    {
        public int Id { get; set; }
        public string ConfigUrl { get; set; }
        public DateTime LastModified { get; set; }
    }
}
