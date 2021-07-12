using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.QueryFilter
{
    public class ModelQueryFilter:BaseQueryFilter
    {
        public bool SuccessList { get; set; } = false;
        public bool FailedList { get; set; } = false;
        public bool QueuedList { get; set; } = false;
        public bool TrainningList { get; set; } = false;
    }
}
