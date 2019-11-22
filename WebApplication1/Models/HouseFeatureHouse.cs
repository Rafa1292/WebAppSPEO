using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class HouseFeatureHouse
    {
        [Key]
        public int HouseFeatureHouseId { get; set; }
        public int HouseFeatureId { get; set; }
        public int HouseId { get; set; }
        public virtual HouseFeature HouseFeature { get; set; }
        public virtual House House { get; set; }
    }
}