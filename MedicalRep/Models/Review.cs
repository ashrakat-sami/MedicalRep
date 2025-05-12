using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalRep.Models
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string DoctorId { get; set; }
        public string ProductId { get; set; }
        public ApplicationUser Doctor { get; set; }
        public Product Product { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
    }
}
