using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AVC.Models
{
    public partial class Account
    {
        public Account()
        {
            AssignedCarAccount = new HashSet<AssignedCar>();
            AssignedCarAssignedByNavigation = new HashSet<AssignedCar>();
            Car = new HashSet<Car>();
            CarConfig = new HashSet<CarConfig>();
            Configuration = new HashSet<Configuration>();
            InverseManagedByNavigation = new HashSet<Account>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public bool? IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ResetPasswordToken { get; set; }
        public int? ManagedBy { get; set; }

        public virtual Account ManagedByNavigation { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<AssignedCar> AssignedCarAccount { get; set; }
        public virtual ICollection<AssignedCar> AssignedCarAssignedByNavigation { get; set; }
        public virtual ICollection<Car> Car { get; set; }
        public virtual ICollection<CarConfig> CarConfig { get; set; }
        public virtual ICollection<Configuration> Configuration { get; set; }
        public virtual ICollection<Account> InverseManagedByNavigation { get; set; }
    }
}
