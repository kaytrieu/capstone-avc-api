using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.QueryFilter
{
    public class BaseQueryFilter
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string SearchValue { get; set; } = "";
    }
}
