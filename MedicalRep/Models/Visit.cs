namespace MedicalRep.Models
{
    public class Visit
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int MedicalRepId { get; set; }

        public DateTime VisitDate { get; set; }
        public string VisitNotes { get; set; }

        public ApplicationUser User { get; set; } // Navigation Property   
        

    }
}
