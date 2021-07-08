using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.HubMessages
{
    public class WhenCarRunningMessage
    {
        public List<int> AccountIdList { get; set; }
        public int CarId { get; set; }

        public WhenCarRunningMessage(List<int> accountIdList, int carId)
        {
            AccountIdList = accountIdList;
            CarId = carId;
        }
    }
}
