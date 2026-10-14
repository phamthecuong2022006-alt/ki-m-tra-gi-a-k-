using Microsoft.AspNetCore.Mvc;

namespace BIT240049_Mid.Controllers;

/// <summary>Controller co so: tra ve trang 404 (kem HTTP 404) cho Id khong ton tai.</summary>
public abstract class AppController : Controller
{
    protected IActionResult NotFoundPage(string messageKey)
    {
        Response.StatusCode = StatusCodes.Status404NotFound;
        return View("~/Views/Shared/NotFound.cshtml", messageKey);
    }
}
