﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ArticlePicture
    {
        [Key]
        public int ArticlePictureId { get; set; }
        public int ArticleId { get; set; }
        public virtual Article Article { get; set; }
    }
}