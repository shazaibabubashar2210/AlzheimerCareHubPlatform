using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace AlzhCareHub.Controllers
{
    public class LanguageController : Controller
    {
        public IActionResult SetLanguage(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            HttpContext.Session.SetString("Culture", culture);
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
