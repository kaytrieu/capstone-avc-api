using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AVC.Dtos.CarDtos
{
    public class CarUpdateDto
    {
        [Required]
        public string Name { get; set; }

    }
}
