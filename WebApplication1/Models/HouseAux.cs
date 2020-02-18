using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class HouseAux
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debes agregar una Medida de frente")]
        [Display(Name = "Medida de frente")]
        public int HouseForeheadMeasureAux { get; set; }
        
        [Required(ErrorMessage = "Debes agregar una Medida de fondo")]
        [Display(Name = "Medida de fondo")]
        public int HouseBackgroundMeasureAux { get; set; }

        [Required(ErrorMessage = "Debes agregar la cantidad de dormitorios")]
        [Display(Name = "dormitorios")]
        public int BedroomsAux { get; set; }

        [Required(ErrorMessage = "Debes agregar la cantidad de baños")]
        [Display(Name = "Baños")]
        public int BathroomsAux { get; set; }

        [Required(ErrorMessage = "Debes indicar el numero de vehiculos")]
        [Display(Name = "Garage")]
        public int GarageAux { get; set; }

        [Required(ErrorMessage = "Debes agregar el numero de niveles")]
        [Display(Name = "Niveles")]
        public int LevelsAux { get; set; }

        public int ArticleId { get; set; }
        public virtual Article Article { get; set; }
        public ICollection<HouseFeatureHouse> HouseFeaturesHouse { get; set; }
    }
}