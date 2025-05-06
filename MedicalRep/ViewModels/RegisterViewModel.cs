using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalRep.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords don't match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LName { get; set; }

        public string? City { get; set; }
        public string? Street { get; set; }
        public string? Government { get; set; }
        public bool IsDeleted { get; set; } = false;

        public int? SpecializationId { get; set; }
        public string? Location { get; set; }

        [Required(ErrorMessage = "Please select a role.")]
        [Display(Name = "User Role")]
        public string SelectedRole { get; set; }

        // Made nullable since it's only for view population
        public List<SelectListItem>? RoleOptions { get; set; }
    }
}