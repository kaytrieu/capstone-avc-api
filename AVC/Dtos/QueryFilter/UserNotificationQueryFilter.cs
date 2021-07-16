using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.QueryFilter
{
    public class UserNotificationQueryFilter
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        [Required]
        public int ReceiverId { get; set; }
    }
}
