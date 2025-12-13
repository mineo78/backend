using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace GamingPlatform.Controllers;

[Route("localization")]
public class LocalizationController : Controller
{
    private static readonly HashSet<string> SupportedCultures = new(StringComparer.OrdinalIgnoreCase)
    {
        "fr-FR",
        "en-US",
        "es-ES",
    };

    [HttpPost("set-language")]
    [ValidateAntiForgeryToken]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        var chosenCulture = SupportedCultures.Contains(culture) ? culture : "fr-FR";

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(chosenCulture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                HttpOnly = false, // readable by client-side if needed; not sensitive
            }
        );

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}


