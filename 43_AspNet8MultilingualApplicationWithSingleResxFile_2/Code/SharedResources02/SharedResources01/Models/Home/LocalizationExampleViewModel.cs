using Microsoft.AspNetCore.Mvc.Localization;

//LocalizationExampleViewModel.cs===============================================
namespace SharedResources02.Models.Home
{
    public class LocalizationExampleViewModel
    {
        public string? IStringLocalizerInController { get; set; }
        public LocalizedHtmlString? IHtmlLocalizerInController { get; set; }
    }
}
