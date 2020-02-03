using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Ubication
    {
        [Key]
        public int UbicationId { get; set; }

        [Display(Name = "Descripcion")]
        [StringLength(1000, ErrorMessage = "Maximo 200 caracteres")]
        public string Description { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(50, ErrorMessage = "Maximo 50 caracteres")]
        [Required(ErrorMessage = "Debes agregar un nombre")]
        public string Name { get; set; }
        public ICollection<UbicationPicture> UbicationPictures { get; set; }
        public ICollection<Article> Articles { get; set; }
        public ICollection<UbicationFeatureUbication> UbicationFeaturesUbication { get; set; }
        public int DistritId { get; set; }
        public virtual Distrit Distrit { get; set; }
        public int UbicationCategoryId { get; set; }
        public virtual UbicationCategory UbicationCategory { get; set; }

    }
}