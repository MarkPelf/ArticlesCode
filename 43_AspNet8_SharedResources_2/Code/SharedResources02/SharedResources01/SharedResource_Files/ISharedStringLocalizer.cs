using Microsoft.Extensions.Localization;

//ISharedStringLocalizer.cs=================================
namespace SharedResources02
{
    //we create this interface to use it for DI dependency setting
    public interface ISharedStringLocalizer
    {
        public LocalizedString this[string key]
        {
            get;
        }

        LocalizedString GetLocalizedString(string key);
    }
}
