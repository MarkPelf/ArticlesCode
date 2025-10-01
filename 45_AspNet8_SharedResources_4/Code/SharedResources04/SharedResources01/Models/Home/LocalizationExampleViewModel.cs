using Microsoft.AspNetCore.Mvc.Localization;

//LocalizationExampleViewModel.cs===============================================
namespace SharedResources04.Models.Home
{
    public class LocalizationExampleViewModel
    {
        public string? LocalizedInControllerByResourceManager1 { get; set; }
        public string? LocalizedInControllerByResourceManager2 { get; set; }

        public string? LocalizedInControllerByIStringLocalizer1 { get; set; }
        public string? LocalizedInControllerByIStringLocalizer2 { get; set; }

        public string? ThreadCultureInController { get; set; }
    }
}
