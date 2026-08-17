using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeBar.WebApp.Compartilhado;

public class HomeController : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public ActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction(nameof(Home));

        return View();
    }

    [HttpGet]
    public ActionResult Home()
    {
        return View();
    }
}