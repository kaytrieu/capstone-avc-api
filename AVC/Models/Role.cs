using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AVC.Models
{
    public partial class Role
    {
        public Role()
        {
            Account = new HashSet<Account>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsAvailable { get; set; }
        public int Id { get; set; }

        public virtual ICollection<Account> Account { get; set; }
    }
}
