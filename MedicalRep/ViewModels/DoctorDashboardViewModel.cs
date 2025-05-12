namespace MedicalRep.ViewModels
{
    public class DoctorDashboardViewModel
    {
        public ApplicationUser Doctor { get; set; }
        public Specialization Specialization { get; set; }
        public List<Product> Products { get; set; }
        public List<Review> RecentReviews { get; set; }
        public List<Visit> UpcomingVisits { get; set; }
        public List<Product> TopRatedProducts { get; set; }
        public int CompletedVisitsCount { get; set; }
    }

  

    
   
    
}
