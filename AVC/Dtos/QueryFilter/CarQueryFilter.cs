using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.QueryFilter
{
    public class CarQueryFilter : BaseQueryFilter
    {
        public bool? IsAvailable { get; set; } = null;
        public bool? IsApproved { get; set; } = null;
    }
}
