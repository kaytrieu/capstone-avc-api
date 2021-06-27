using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.QueryFilter
{
    public class IssueQueryFilter : BaseQueryFilter
    {
        public int? CarId { get; set; } = null;
        public int? TypeId { get; set; } = null;
    }
}
