using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Constant
{
    public static class ModelState
    {
        public const string Queued = "Queued";
        public const string Trainning = "Trainning";
        public const string Successed = "Successed";
        public const string Failed = "Failed";        
        public const int QueuedId = 1;
        public const int TrainningId = 2;
        public const int SuccessedId = 3;
        public const int FailedId = 4;
    }

}
