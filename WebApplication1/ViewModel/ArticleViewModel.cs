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
        public House House { get; set; }
        public HouseAux HouseAux { get; set; }
        public List<ArticlePicture> Pictures { get; set; }
        public ArticlePicture OutstandingPicture { get; set; }



    }
}