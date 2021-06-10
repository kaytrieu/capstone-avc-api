using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AVC.Models
{
    public partial class ModelVersion
    {
        public ModelVersion()
        {
            Car = new HashSet<Car>();
        }

        public int Id { get; set; }
        public string ModelUrl { get; set; }
        public double? Loss { get; set; }
        public double? Map { get; set; }
        public bool? IsAvailable { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string StatisticUrl { get; set; }
        public int? EpochCount { get; set; }
        public int? ImageCount { get; set; }
        public int ModelStatusId { get; set; }
        public string Name { get; set; }
        public string ImageFolderUrl { get; set; }

        public virtual ModelStatus ModelStatus { get; set; }
        public virtual ICollection<Car> Car { get; set; }
    }
}
