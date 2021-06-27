using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AVC.Dtos.IssueDtos
{
    public class IssueCreateDto
    {
        public int TypeId { get; set; }
        public int CarId { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
    }
}
