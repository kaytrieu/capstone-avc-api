using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AVC.Dtos.CarDtos
{
    public class CarApprovalDto
    {
        [Required]
        public bool IsApproved { get; set; }
        public IFormFile ImageFile { get; set; }
        public string Name { get; set; }
        public IFormFile ConfigFile { get; set; }
        public int? ManagedBy { get; set; }

    }
}
