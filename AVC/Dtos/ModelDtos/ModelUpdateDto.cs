using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.ModelDtos
{
    public class ModelUpdateDto
    {
        [Required]
        public double? Loss { get; set; }
        [Required]
        public double? Map { get; set; }
        [Required]
        public string StatisticUrl { get; set; }
    }
}
