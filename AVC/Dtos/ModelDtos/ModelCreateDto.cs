using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.ModelDtos
{
    public class ModelCreateDto
    {
        public IFormFile zipFile { get; set; }
        public string Name { get; set; }
        public int? ImageCount { get; set; }
    }
}
