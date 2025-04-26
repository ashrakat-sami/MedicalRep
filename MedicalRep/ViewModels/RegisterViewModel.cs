
namespace MedicalRep.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        // Add Role (drop down list )
       
        //[Display(Name = "Role")]
        //public string? RoleName { get; set; }

        //// For populating dropdown list
        //public List<SelectListItem>? Roles { get; set; }






    }
}
