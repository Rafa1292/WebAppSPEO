using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Distrit
    {
       [Key]
        public int DistritId { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(20, ErrorMessage = "Maximo 20 caracteres")]
        [Required(ErrorMessage = "Debes agregar un nombre")]
        public string Name { get; set; }
        public virtual Canton Canton { get; set; }
        public int CantonId { get; set; }

        public ICollection<Ubication> Ubications { get; set; }
    }
}