using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalRep.Models
{
    public class Visit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string DoctorId { get; set; }  // This can be related to another User or doctor, depending on your business logic
        public string  MedicalRepId { get; set; }  // This references a Medical Representative

        public DateTime VisitDate { get; set; }
        public string VisitNotes { get; set; }

        public ApplicationUser User { get; set; }  // Navigation Property to User (Medical Rep or Doctor)
    }

}
