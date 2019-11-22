using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class UbicationFeatureUbication
    {
        [Key]
        public int UbicationFeatureUbicationId { get; set; }
        public int UbicationFeatureId { get; set; }
        public int UbicationId { get; set; }
        public virtual UbicationFeature UbicationFeature { get; set; }
        public virtual Ubication Ubication { get; set; }
    }
}