   using System.ComponentModel.DataAnnotations;

    namespace MedicalRep.ViewModels
    {
        public class VisitViewModel
        {
            public string? DoctorId { get; set; }
            public bool IsNewDoctor { get; set; } = false;

            // New doctor fields
            [Display(Name = "First Name")]
            public string? NewDoctorFirstName { get; set; }

            [Display(Name = "Last Name")]
            public string? NewDoctorLastName { get; set; }

            [Display(Name = "Email")]
            [EmailAddress]
            public string? NewDoctorEmail { get; set; }

            [Display(Name = "Phone Number")]
            public string? NewDoctorPhone { get; set; }

            [Display(Name = "Clinic Location")]
            public string? NewDoctorLocation { get; set; }

            [Display(Name = "Specialization")]
            public int? NewDoctorSpecializationId { get; set; }

            // Visit fields
            [Required]
            [Display(Name = "Visit Date")]
            public DateTime VisitDate { get; set; }

            [Display(Name = "Visit Notes")]
            public string? VisitNotes { get; set; }
        }

        public class VisitEditViewModel
        {
            public int Id { get; set; }
            public string? DoctorId { get; set; }
            public string? DoctorName { get; set; }

            [Required]
            [Display(Name = "Visit Date")]
            public DateTime VisitDate { get; set; }

            [Display(Name = "Actual Visit Date")]
            public DateTime? ActualVisitDate { get; set; }

            [Display(Name = "Visit Notes")]
            public string? VisitNotes { get; set; }

            [Display(Name = "Feedback")]
            public string? Feedback { get; set; }

            [Display(Name = "Status")]
            public VisitStatus Status { get; set; }
        }
    }

