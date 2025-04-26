namespace MedicalRep.Models
{
    public class Review
    {
        public int DoctorId { get; set; }  // Foreign Key to Doctor
        public int ProductId { get; set; } // Foreign Key to Product
        public ApplicationUser Doctor { get; set; } // Navigation Property
        public Product Product { get; set; } // Navigation Property

        public int Rating { get; set; }     // e.g., 1-5
        public string Comment { get; set; } // Optional
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
    }
}
