using Microsoft.AspNetCore.Mvc.Localization;
using System.ComponentModel.DataAnnotations;

//LocalizationExampleViewModel.cs===============================================
namespace SharedResources03.Models.Home
{    public class LocalizationExampleViewModel
    {
        /* It is these field validation error messages
         * that are focus of this example. We want to
         * be able to present them in multiple languages
         */
        //model
        [Required(ErrorMessage = "The UserName field is required.")]
        [Display(Name = "UserName")]
        public string? UserName { get; set; }

        [EmailAddress(ErrorMessage = "The Email field is not a valid email address.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        public bool IsSubmit { get; set; } = false;
    }
}
