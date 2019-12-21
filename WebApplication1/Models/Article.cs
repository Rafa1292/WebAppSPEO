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
        [Key]
        public int ArticleId { get; set; }

        [Display(Name = "Descripcion")]
        [StringLength(200,ErrorMessage = "Maximo 200 caracteres")]
        [Required(ErrorMessage = "Debes agregar una descripcion")]
        public string Description { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "Debes agregar un codigo")]
        public bool State { get; set; }

        [Display(Name = "Codigo de articulo")]
        public string Code { get; set; }

        public string Currency { get; set; }

        [Display(Name = "Precio")]
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "Debes agregar un precio")]
        public decimal Price { get; set; }
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