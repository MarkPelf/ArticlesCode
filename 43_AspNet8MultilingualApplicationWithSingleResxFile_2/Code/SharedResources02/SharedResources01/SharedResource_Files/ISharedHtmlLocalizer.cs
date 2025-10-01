using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;

//ISharedHtmlLocalizer.cs===============================================
namespace SharedResources02
{
    //we create this interface to use it for DI dependency setting
    public interface ISharedHtmlLocalizer
    {
        public LocalizedHtmlString this[string key]
        {
            get;
        }

        LocalizedHtmlString GetLocalizedString(string key);
    }
}
