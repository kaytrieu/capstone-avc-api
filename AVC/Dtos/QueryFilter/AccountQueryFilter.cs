using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.QueryFilter
{
    public class AccountQueryFilter : BaseQueryFilter
    {
        public bool? IsAvailable { get; set; } = null;
    }
}
