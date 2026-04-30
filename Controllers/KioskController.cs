using Microsoft.AspNetCore.Mvc;

namespace BankTurns.Controllers;

public class KioskController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
