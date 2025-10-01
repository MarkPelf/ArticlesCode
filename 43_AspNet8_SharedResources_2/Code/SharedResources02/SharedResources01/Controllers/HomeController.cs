using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using SharedResources02.Models;
using SharedResources02.Models.Home;
using System.Diagnostics;
using SharedResources02;
using Microsoft.AspNetCore.Mvc.Localization;
using System.Xml.Linq;

//HomeController.cs================================================================
namespace SharedResources02.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISharedStringLocalizer _stringLocalizer;
        private readonly ISharedHtmlLocalizer _htmlLocalizer;

        /* Here is of course the Dependency Injection (DI) coming in and filling 
         * all the dependencies. 
         * So, here will services SharedStringLocalizer and SharedHtmlLocalizer
         * be injected
         */
        public HomeController(ILogger<HomeController> logger,
            ISharedStringLocalizer stringLocalizer,
            ISharedHtmlLocalizer htmlLocalizer)
        {
            _logger = logger;
            _stringLocalizer = stringLocalizer;
            _htmlLocalizer = htmlLocalizer;
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
            myContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) }
            );
        }

        public IActionResult LocalizationExample(LocalizationExampleViewModel model)
        {
            //so, here we use ISharedStringLocalizer
            model.IStringLocalizerInController = _stringLocalizer["Wellcome"];
            //so, here we use ISharedHtmlLocalizer
            model.IHtmlLocalizerInController = _htmlLocalizer["Wellcome"];
            return View(model);
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
