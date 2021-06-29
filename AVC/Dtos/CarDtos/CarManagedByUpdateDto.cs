using AVC.Dtos.AccountDtos;
using AVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.CarDtos
{
    public class CarManagedByUpdateDto
    {
        public int CarId { get; set; }
        public int ManagerId { get; set; }
    }
}
