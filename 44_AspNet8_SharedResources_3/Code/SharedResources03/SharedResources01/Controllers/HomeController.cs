using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using SharedResources03.Models;
using SharedResources03.Models.Home;
using System.Diagnostics;
using SharedResources03;
using Microsoft.AspNetCore.Mvc.Localization;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

//HomeController.cs================================================================
namespace SharedResources03.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;

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
            if (culture == null) { throw new Exception("culture == null"); };

            //this code sets .AspNetCore.Culture cookie
            CookieOptions cookieOptions=new CookieOptions();
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
            if(model.IsSubmit)
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", _stringLocalizer["Please correct all errors and submit again"]);
                }
            }
            else
            {
                ModelState.Clear();
            }

            return View(model);
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
