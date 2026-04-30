using Microsoft.AspNetCore.Mvc;

namespace BankTurns.Controllers;

public class WaitingRoomController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
