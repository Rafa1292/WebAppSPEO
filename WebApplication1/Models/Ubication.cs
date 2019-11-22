﻿using System;
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
        [StringLength(50, ErrorMessage = "Maximo 50 caracteres")]
        [Required(ErrorMessage = "Debes agregar un nombre")]
        public string Name { get; set; }
        public ICollection<UbicationPicture> UbicationPictures { get; set; }
        public ICollection<Article> Articles { get; set; }
        public ICollection<UbicationFeatureUbication> UbicationFeaturesUbication { get; set; }
        public int CantonId { get; set; }
        public virtual Canton Canton { get; set; }
    }
}