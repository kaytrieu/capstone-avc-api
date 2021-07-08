using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.HubMessages
{
    public class WhenCarStoppingMessage
    {
        public List<int> AccountIdList { get; set; }
        public int CarId { get; set; }

        public WhenCarStoppingMessage(List<int> accountIdList, int carId)
        {
            AccountIdList = accountIdList;
            CarId = carId;
        }
    }
}
