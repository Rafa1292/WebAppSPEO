using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Reject
    {
        [Key]
        public int RejectId { get; set; }

        public string  Reason { get; set; }

        public virtual Article Article { get; set; }

        public int ArticleId { get; set; }

    }
}