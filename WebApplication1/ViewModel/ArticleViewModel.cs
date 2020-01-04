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
        public int[] HouseAuxFeatures { get; set; }
        public int[] TerrainFeatures { get; set; }
        public string OutstandingPicture { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public int IndividualContributorId { get; set; }
        public int UbicationId { get; set; }


    }
}