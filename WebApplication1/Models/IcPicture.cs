using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class IcPicture
    {
        [Key]
        public int IcPictureId { get; set; }
        public int IndividualContributorId { get; set; }
        public virtual IndividualContributor IndividualContributor { get; set; }
    }
}