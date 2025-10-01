using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.VisualBasic;

//Program.cs===========================================================================
namespace SharedResources03
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //=====Middleware and Services=============================================
            var builder = WebApplication.CreateBuilder(args);

            //adding multi-language support
            AddingMultiLanguageSupportServices(builder);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //====App===================================================================
            var app = builder.Build();

            //adding multi-language support
            AddingMultiLanguageSupport(app);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=ChangeLanguage}/{id?}");

            app.Run();
        }

        private static void AddingMultiLanguageSupportServices(WebApplicationBuilder? builder)
        {
            if (builder == null) { throw new Exception("builder==null"); };

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddMvc()
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                    .AddDataAnnotationsLocalization(options =>
                    {
                        options.DataAnnotationLocalizerProvider = (type, factory) =>
                            factory.Create(typeof(SharedResource));
                    });

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "en", "fr", "de", "it" };
                options.SetDefaultCulture(supportedCultures[0])
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures);
            });
        }

        private static void AddingMultiLanguageSupport(WebApplication? app)
        {
            app?.UseRequestLocalization();
        }
    }
}
