using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Article
    {
        public int Id { get; set; }

        [Display(Name = "Descripcion")]
        [StringLength(200,ErrorMessage = "Maximo 200 caracteres")]
        public string Description { get; set; }

        [Display(Name = "Estado")]
        public bool State { get; set; }

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
    }
}