using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalRep.Models
{
    public class Specialization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<ApplicationUser> Doctors { get; set; } = new List<ApplicationUser>();

        public virtual ICollection<ProductSpecialization> ProductSpecializations { get; set; }
    }
}