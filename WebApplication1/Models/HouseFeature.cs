﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class HouseFeature
    {
        [Key]
        public int HouseFeatureId { get; set; }

        [Display(Name = "Descripcion")]
        [StringLength(50, ErrorMessage = "Maximo 50 caracteres")]
        [Required(ErrorMessage = "Debes agregar una descripcion")]
        public string Description { get; set; }
        public ICollection<HouseFeatureHouse> HouseFeaturesHouse { get; set; }
    }
}