namespace MedicalRep.ViewModels
{
    public class MedicalRepDashboardViewModel
    {
        public ApplicationUser MedicalRep { get; set; }
        public List<Product> Products { get; set; }
        public List<DoctorsBySpecialization> DoctorsBySpecialization { get; set; }
        public List<Visit> RecentVisits { get; set; }
        public VisitStats VisitStats { get; set; }
        public int TotalAllDoctors { get; set; }

        // Helper properties
        public int TotalSpecializations => DoctorsBySpecialization?.Count ?? 0;
        public int TotalProducts => Products?.Count ?? 0;
    }

    public class DoctorsBySpecialization
    {
        public Specialization Specialization { get; set; }
        public int DoctorCount { get; set; }
    }

    public class VisitStats
    {
        public int TotalVisits { get; set; }
        public int ThisMonthVisits { get; set; }
        public int PlannedVisits { get; set; }
    }

    public class VisitPlannerViewModel
    {
        public List<ApplicationUser> Doctors { get; set; }
        public List<Visit> PlannedVisits { get; set; }
        public DateTime? SelectedDate { get; set; }
        public string SelectedDoctorId { get; set; }
        public string VisitNotes { get; set; }
    }

    public class SearchResultViewModel
    {
        public List<DoctorSearchResult> Doctors { get; set; }
        public List<ProductSearchResult> Products { get; set; }
    }

    public class DoctorSearchResult
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public string Location { get; set; }
    }

    public class ProductSearchResult
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }
        public string Description { get; set; }
    }
}