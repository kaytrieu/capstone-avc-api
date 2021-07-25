using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.ModelDtos
{
    public class ModelUpdateDto
    {
        public double? Loss { get; set; }
        public double? Map { get; set; }
        public string StatisticUrl { get; set; }
    }
}
