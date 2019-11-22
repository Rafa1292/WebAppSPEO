using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class IndividualContributorPicture
    {
        [Key]
        public int IndividualContributorPictureId { get; set; }
        public int IndividualContributorId { get; set; }
        public virtual IndividualContributor IndividualContributor { get; set; }
    }
}