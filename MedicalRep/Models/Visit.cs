using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalRep.Models
{
    public class Visit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string DoctorId { get; set; }

        [Required]
        public string MedicalRepId { get; set; }

        [Required]
        public DateTime VisitDate { get; set; }

        public string? VisitNotes { get; set; }

        [Required]
        public VisitStatus Status { get; set; } = VisitStatus.Scheduled;

        [ForeignKey("DoctorId")]
        public virtual ApplicationUser Doctor { get; set; }

        [ForeignKey("MedicalRepId")]
        public virtual ApplicationUser MedicalRep { get; set; }

        public DateTime? ActualVisitDate { get; set; }
        public string? Feedback { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    public enum VisitStatus
    {
        Scheduled = 1,     // مجدولة
        Completed = 2,     // مكتملة
        Cancelled = 3,     // ملغاة
        Rescheduled = 4,   // تم إعادة جدولتها
        NoShow = 5,        // الطبيب لم يحضر
        InProgress = 6     // قيد التنفيذ (للمتابعة الفورية)
    }

}
