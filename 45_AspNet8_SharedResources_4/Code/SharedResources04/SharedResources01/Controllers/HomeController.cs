using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using SharedResources04.Models;
using SharedResources04.Models.Home;
using System.Diagnostics;
using SharedResources04;
using Microsoft.AspNetCore.Mvc.Localization;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

//HomeController.cs================================================================
namespace SharedResources04.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;

        /* Here is, of course, the Dependency Injection (DI) coming in and filling 
         * all the dependencies. The key thing is we are asking for a specific 
         * type=SharedResource. 
         * If it doesn't work for you, you can try to use full class name
         * in your DI instruction, like this one:
         * IStringLocalizer<SharedResources04.SharedResource> stringLocalizer
         */
        public HomeController(ILogger<HomeController> logger,
            IStringLocalizer<SharedResource> stringLocalizer)
        {
            _logger = logger;
            _stringLocalizer = stringLocalizer;
        }

        public IActionResult ChangeLanguage(ChangeLanguageViewModel model)
        {
            if (model.IsSubmit)
            {
                HttpContext myContext = this.HttpContext;
                ChangeLanguage_SetCookie(myContext, model.SelectedLanguage);
                //doing funny redirect to get new Request Cookie
                //for presentation
                return LocalRedirect("/Home/ChangeLanguage");
            }

            //prepare presentation
            ChangeLanguage_PreparePresentation(model);
            return View(model);
        }

        private void ChangeLanguage_PreparePresentation(ChangeLanguageViewModel model)
        {
            model.ListOfLanguages = new List<SelectListItem>
                        {
                            new SelectListItem
                            {
                                Text = "English",
                                Value = "en"
                            },

                            new SelectListItem
                            {
                                Text = "German",
                                Value = "de",
                            },

                            new SelectListItem
                            {
                                Text = "French",
                                Value = "fr"
                            },

                            new SelectListItem
                            {
                                Text = "Italian",
                                Value = "it"
                            }
                        };
        }

        private void ChangeLanguage_SetCookie(HttpContext myContext, string? culture)
        {
            if(culture == null) { throw new Exception("culture == null"); };

            //this code sets .AspNetCore.Culture cookie
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTimeOffset.UtcNow.AddMonths(1);
            cookieOptions.IsEssential = true;

            myContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                cookieOptions
            );
        }

        public IActionResult LocalizationExample(LocalizationExampleViewModel model)
        {
            string text = "Thread CurrentUICulture is [" + @Thread.CurrentThread.CurrentUICulture.ToString() + "] ; ";
            text += "Thread CurrentCulture is [" + @Thread.CurrentThread.CurrentCulture.ToString() + "]";
            model.ThreadCultureInController = text;
            //here we test localization by Resource Manager
            model.LocalizedInControllerByResourceManager1 = Resources.SharedResource.Wellcome;
            model.LocalizedInControllerByResourceManager2 = Resources.SharedResource.Hello_World;
            //here we test localization by IStringLocalizer
            model.LocalizedInControllerByIStringLocalizer1 = _stringLocalizer["Wellcome"];
            model.LocalizedInControllerByIStringLocalizer2 = _stringLocalizer["Hello World"];

            return View(model);
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
