using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class House
    {
        [Key]
        public int HouseId { get; set; }

        [Display(Name = "Descripcion")]
        [StringLength(200, ErrorMessage = "Maximo 200 caracteres")]
        [Required(ErrorMessage = "Debes agregar una descripcion")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Debes agregar una Medida de frente")]
        [Display(Name = "Medida de frente")]
        public int ForeheadMeasure { get; set; }

        [Required(ErrorMessage = "Debes agregar una Medida de fondo")]
        [Display(Name = "Medida de fondo")]
        public int BackgroundMeasure { get; set; }

        [Required(ErrorMessage = "Debes agregar la cantidad de dormitorios")]
        [Display(Name = "dormitorios")]
        public int Bedrooms { get; set; }

        [Required(ErrorMessage = "Debes agregar la cantidad de baños")]
        [Display(Name = "Baños")]
        public int Bathrooms { get; set; }

        [Required(ErrorMessage = "Debes indicar el numero de vehiculos")]
        [Display(Name = "Garage")]
        public int Garage { get; set; }

        [Required(ErrorMessage = "Debes agregar el numero de niveles")]
        [Display(Name = "Niveles")]
        public int Levels { get; set; }
        public int ArticleId { get; set; }
        public virtual Article Article { get; set; }
        public ICollection<HouseFeatureHouse> HouseFeaturesHouse { get; set; }

    }
}