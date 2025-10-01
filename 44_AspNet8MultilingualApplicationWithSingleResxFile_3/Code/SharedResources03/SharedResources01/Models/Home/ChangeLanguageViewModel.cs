using Microsoft.AspNetCore.Mvc.Rendering;

//ChangeLanguageViewModel.cs=====================================================
namespace SharedResources03.Models.Home
{
    public class ChangeLanguageViewModel
    {
        //model
        public string? SelectedLanguage { get; set; } = "en";

        public bool IsSubmit { get; set; } = false;

        //view model
        public List<SelectListItem>? ListOfLanguages { get; set; }
    }
}
