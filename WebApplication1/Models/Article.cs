﻿using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Models
{
    public class Article
    {
        public int Id { get; set; }

        [Display(Name = "Descripcion")]
        public string Description { get; set; }

        [Display(Name = "Propietario")]
        public string OwnerName { get; set; }

        [Display(Name = "Contacto propietario")]
        public int OwnerPhone { get; set; }

        [Display(Name = "Estado")]
        public bool State { get; set; }

        public DateTime CreationDate { get; set; }

        public EArticleKind ArticleKind { get; set; }
        public bool SoldState { get; set; }

        [Display(Name = "Codigo de articulo")]
        public string Code { get; set; }

        public string Currency { get; set; }

        [Display(Name = "Precio")]
        [Required(ErrorMessage = "Debes agregar un precio")]
        public string Price { get; set; }
        public virtual Terrain Terrain { get; set; }
        public int TerrainId { get; set; }
        public virtual Ubication Ubication { get; set; }
        public int UbicationId { get; set; }
        public virtual IndividualContributor IndividualContributor { get; set; }
        public int IndividualContributorId { get; set; }
        public ICollection<ArticleClient> ArticlesClient { get; set; }
        public ICollection<ArticlePicture> ArticlesPicture { get; set; }


        public int RefreshApproves()
        {
            using (WebApplication1Context db = new WebApplication1Context())
            {
                var rejects = db.Rejects.ToList();

                var approves = from a in db.Articles
                               where !a.State
                               select a;

                var result = approves.ToList().Count() - rejects.Count();

                return result;
            }
        }

        public int RefreshRejects(string userMail)
        {
            using (WebApplication1Context db = new WebApplication1Context())
            {
                var rejects = from r in db.Rejects
                              where r.Article.IndividualContributor.Mail == userMail
                              select r;
                var rejectsList = rejects.ToList();
                var result = rejects.Count();

                return result;
            }
        }
    }
}