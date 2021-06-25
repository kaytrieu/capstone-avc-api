using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.ReponseDtos
{
    public class ResponseDto
    {
        public string Message { get; set; }

        public ResponseDto(string message)
        {
            Message = message;
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
