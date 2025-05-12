using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalRep.Models
{
    public class ApplicationUser : IdentityUser
    {
   

        public string? FName { get; set; }
        public string? LName { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? Government { get; set; }
        public bool IsDeleted { get; set; } = false;
       
        // for Drs only 
        public int? SpecializationId { get; set; }
        public Specialization? Specialization { get; set; }
        public string? Location { get; set; } // for Doctor and MedicalRep => Doctor clinic Location indetails

        public ICollection<Review>? Reviews { get; set; } = new List<Review>();

        public string PhoneNumber { get; set; } = string.Empty;


        public virtual ICollection<Visit> DoctorVisits { get; set; } = new List<Visit>();

        // Visits where this user is the Medical Representative
        public virtual ICollection<Visit> MedicalRepVisits { get; set; } = new List<Visit>();
    }
}