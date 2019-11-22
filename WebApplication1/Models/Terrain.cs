using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Terrain
    {
        [Key]
        public int TerrainId { get; set; }

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

        [Required(ErrorMessage = "Debes describir la topografia del terreno")]
        [Display(Name = "Topografia")]
        public string Topography { get; set; }
        public ICollection<TerrainFeatureTerrain> TerrainFeaturesTerrain { get; set; }
    }
}