using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Blog
    {
        [Key]
        public int BlogId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public byte CoverImageArray { get; set; }

        public byte VideoArray { get; set; }
    }
}