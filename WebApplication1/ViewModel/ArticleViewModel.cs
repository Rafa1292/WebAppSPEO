using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.ViewModel
{
    public class ArticleViewModel
    {
        public Article Article { get; set; }
        public Terrain Terrain { get; set; }
        public House House { get; set; }
        public HouseAux HouseAux { get; set; }
        public string[] Urls { get; set; }
        public int[] HouseFeatures { get; set; }
        public int[] HouseFeturesAux { get; set; }
        public int[] TerrainFeatures { get; set; }
        public string outstandingPicture { get; set; }

    }
}