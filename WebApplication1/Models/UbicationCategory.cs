using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class UbicationCategory
    {
        [Key]
        public int UbicationCategoryId { get; set; }

        [Display(Name = "Nombre de categoria")]
        [StringLength(20, ErrorMessage = "Maximo 20 caracteres")]
        [Required(ErrorMessage = "Debes agregar un nombre")]
        public string Name { get; set; }
        public ICollection<Ubication> Ubications { get; set; }
    }
}