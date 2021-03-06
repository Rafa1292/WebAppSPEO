﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class IndividualContributor
    {
        [Key]
        public int IndividualContributorId { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(20, ErrorMessage = "Maximo 20 caracteres")]
        [Required(ErrorMessage = "Debes agregar un nombre")]
        public string Name { get; set; }

        [RegularExpression("[0-9]{4}\\-[0-9]{4}", ErrorMessage = "Debes digitar los primeros 4 digitos seguidos de un guion(-) y los ultimos 4 digitos")]
        [Display(Name = "Telefono", Prompt = "0000-0000")]
        [Required(ErrorMessage = "Debes agregar un numero de telefono")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Identificacion")]
        [Required(ErrorMessage = "Debes agregar un numero de identificacion")]
        public int Identification { get; set; }
        public byte[] PictureArray { get; set; }

        [Display(Name = "Correo")]
        [Required(ErrorMessage = "Debes agregar un correo")]
        [EmailAddress(ErrorMessage = "Direccion  de correo invalida")]
        public string Mail { get; set; }
        public ICollection<Article> Articles { get; set; }
    }
}