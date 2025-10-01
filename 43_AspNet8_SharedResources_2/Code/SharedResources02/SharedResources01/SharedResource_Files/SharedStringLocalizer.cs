using Microsoft.Extensions.Localization;
using System.Reflection;

//SharedStringLocalizer.cs==================================================
namespace SharedResources02
{
    //we create this helper/wrapper class/service
    //that we are going to pass around in DI
    public class SharedStringLocalizer : ISharedStringLocalizer
    {
        //here is object that is doing the real work
        //it is almost the same as IStringLocalizer<SharedResource>
        private readonly IStringLocalizer localizer;

        public SharedStringLocalizer(IStringLocalizerFactory factory)
        {
            var type = typeof(SharedResource);
            var assemblyName = new AssemblyName(
                type.GetTypeInfo().Assembly.FullName ?? String.Empty);
            this.localizer = factory.Create("SharedResource", 
                assemblyName?.Name ?? String.Empty);
        }

        //in our methods we just pass work to internal object
        public LocalizedString this[string key] => this.localizer[key];

        public LocalizedString GetLocalizedString(string key)
        {
            return this.GetLocalizedString(key);
        }
    }
}
