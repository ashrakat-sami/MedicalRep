using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalRep.Models
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string DoctorId { get; set; }  // Foreign Key to Doctor
        public string ProductId { get; set; } // Foreign Key to Product
        public ApplicationUser Doctor { get; set; } // Navigation Property
        public Product Product { get; set; } // Navigation Property
        public string Comment { get; set; } // Optional
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
    }
}
