using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ContentType
    {
        [Key]
        public int ContentTypeId { get; set; }
        public string Description { get; set; }
        public ICollection<TerrainFeature> TerrainFeatures { get; set; }
        public ICollection<HouseFeature> HouseFeatures { get; set; }
        public ICollection<UbicationFeature> UbicationFeatures { get; set; }

    }
}