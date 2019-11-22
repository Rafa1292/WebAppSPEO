using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class UbicationPicture
    {
        [Key]
        public int UbicationPictureId { get; set; }
        public int UbicationId { get; set; }

    }
}