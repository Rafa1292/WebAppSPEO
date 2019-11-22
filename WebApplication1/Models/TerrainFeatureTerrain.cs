using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class TerrainFeatureTerrain
    {
        [Key]
        public int TerrainFeatureTerrainId { get; set; }
        public int TerrainId { get; set; }
        public int TerrainFeatureId { get; set; }
        public virtual Terrain Terrain { get; set; }
        public virtual TerrainFeature TerrainFeature { get; set; }
    }
}