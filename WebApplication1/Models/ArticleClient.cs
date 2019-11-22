using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ArticleClient
    {
        [Key]
        public int ArticleClientId { get; set; }
        public virtual Article Article { get; set; }
        public int ArticleId { get; set; }
        public virtual Client Client { get; set; }
        public int ClientId { get; set; }
    }
}