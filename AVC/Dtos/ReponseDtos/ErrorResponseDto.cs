using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.ReponseDtos
{
    public class ErrorResponseDto
    {
        public string Message { get; set; }

        public ErrorResponseDto(string message)
        {
            Message = message;
        }
    }
}
