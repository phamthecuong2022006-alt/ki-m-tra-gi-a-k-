using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace BIT240049_Mid.Controllers;

public class HomeController : AppController
{
    public IActionResult Index() => RedirectToAction("Index", "Dishes");

    // Doi ngon ngu: luu vao cookie roi quay lai dung URL cu (kem query string => giu dieu kien loc)
    [HttpGet]
    public IActionResult SetLanguage(string culture, string? returnUrl)
    {
        var ui = culture == "vi" ? "vi" : "en";

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("en-US", ui)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            });

        if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
        {
            returnUrl = "/";
        }
        return LocalRedirect(returnUrl);
    }

    // Trang loi cho cac URL khong ton tai (/abc, /Dishes/Foo ...)
    public IActionResult HttpStatus(int code)
    {
        if (code == StatusCodes.Status404NotFound)
        {
            return NotFoundPage("PageNotFound");
        }
        Response.StatusCode = code;
        return View("Error");
    }

    public IActionResult Error()
    {
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        return View();
    }
}
