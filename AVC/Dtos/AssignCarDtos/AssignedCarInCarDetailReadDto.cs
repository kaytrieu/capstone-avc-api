using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AVC.Models
{
    public  class AssignedCarInCarDetailReadDto
    {
        public bool? IsAvailable { get; set; }
        public DateTime AssignedAt { get; set; }
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string AccountEmail { get; set; }
        public string AccountName { get; set; }
        public int AssignedBy { get; set; }
        public string AssignedByEmail { get; set; }
        public string AssignedByName { get; set; }
        public DateTime? RemoveAt { get; set; }
    }
}
