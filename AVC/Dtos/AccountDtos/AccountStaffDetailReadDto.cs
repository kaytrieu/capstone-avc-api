using AVC.Dtos.CarDtos;
using AVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.AccountDtos
{
    public class AccountStaffDetailReadDto
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

        public virtual AccountNotManagedByReadDto ManagedBy { get; set; }

        private ICollection<CarAssignedReadDto> assignedCars;

        public virtual ICollection<CarAssignedReadDto> AssignedCars
        {
            get { return assignedCars.OrderByDescending(x => x.CreatedAt).ToList(); }
            set { assignedCars = value; }
        }
    }
}
