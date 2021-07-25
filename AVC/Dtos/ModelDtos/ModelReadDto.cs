using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.ModelDtos
{
    public class ModelReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ModelUrl { get; set; }
        public double? Loss { get; set; }
        public double? Map { get; set; }
        public int? ImageCount { get; set; }
        public string StatisticUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ModelStatus { get; set; }
        public bool IsApplying { get; set; }


    }
}
