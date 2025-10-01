using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using System.Reflection;

//SharedHtmlLocalizer.cs==================================================
namespace SharedResources02
{
    //we create this helper/wrapper class/service
    //that we are going to pass around in DI
    public class SharedHtmlLocalizer: ISharedHtmlLocalizer
    {
        //here is object that is doing the real work
        //it is almost the same as IHtmlLocalizer<SharedResource>
        private readonly IHtmlLocalizer localizer;

        public SharedHtmlLocalizer(IHtmlLocalizerFactory factory)
        {
            var type = typeof(SharedResource);
            var assemblyName = new AssemblyName(
                type.GetTypeInfo().Assembly.FullName ?? String.Empty);
            this.localizer = factory.Create("SharedResource", 
                assemblyName?.Name ?? String.Empty);
        }

        //in our methods we just pass work to internal object
        public LocalizedHtmlString this[string key] => this.localizer[key];

        public LocalizedHtmlString GetLocalizedString(string key)
        {
            return this.GetLocalizedString(key);
        }
    }
}
