using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.CarDtos
{
    public class CarActivationDto
    {
        [Required]
        public bool IsAvailable { get; set; }

    }
}
