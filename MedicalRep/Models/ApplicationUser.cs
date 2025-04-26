using Microsoft.AspNetCore.Identity;

namespace MedicalRep.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int Id { get; set; }
        public string? FName { get; set; }
        public string? LName { get; set; }
        public int? Age { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? Government { get; set; }
        public bool IsDeleted { get; set; } = false;

        // تحديد نوع المستخدم (Doctor, Admin, MedicalRep)
        public bool doctor { get; set; } = false;
        public bool medicalRep { get; set; } = false;
        public bool Admin { get; set; } = false;

        // الخصائص الخاصة بالطبيب (تترك null لغير الأطباء)
        public int? SpecializationId { get; set; }
        public Specialization? Specialization { get; set; }
        public string? Location { get; set; }// for Doctor and MedicalRep



        // الخصائص الخاصة بالمشرف (تترك null لغير المشرفين)
        public decimal? Salary { get; set; }

        // العلاقة مع التقييمات (Reviews)
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Visit> Visits { get; set; } = new List<Visit>();
    }
}