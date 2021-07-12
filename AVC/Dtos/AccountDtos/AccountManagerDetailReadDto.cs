using AVC.Dtos.CarDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.AccountDtos
{
    public class AccountManagerDetailReadDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public bool? IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }

        private ICollection<AccountNotManagedByReadDto> assignedStaffs;

        public ICollection<AccountNotManagedByReadDto> AssignedStaffs
        {
            get { return assignedStaffs.OrderByDescending(x => x.CreatedAt).ToList(); }
            set { assignedStaffs = value; }
        }

        private ICollection<CarAssignedReadDto> assignedCars;

        public virtual ICollection<CarAssignedReadDto> AssignedCars
        {
            get { return assignedCars.OrderByDescending(x => x.CreatedAt).ToList(); }
            set { assignedCars = value; }
        }

    }
}
