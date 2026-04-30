using Microsoft.AspNetCore.Mvc;

namespace BankTurns.Controllers;

public class AdvisorPanelController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Panel()
    {
        return View();
    }
}
