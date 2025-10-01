using Microsoft.AspNetCore.Mvc.Rendering;

namespace Example04.Models.Home
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
