using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.CarDtos
{
    public class CarPatchDto
    {
        public string Image { get; set; }
        public string Name { get; set; }
        public string ConfigUrl { get; set; }
        public int ManagedBy { get; set; }
    }
}
