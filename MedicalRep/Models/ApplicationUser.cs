using Microsoft.AspNetCore.Identity;

namespace MedicalRep
{
    public class ApplicationUser:IdentityUser
    {
        int Id { get; set; }

        public string? FName { get; set; }
        public string? LName { get; set; }
        public int? Age { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? Government { get; set; }




    }
}
