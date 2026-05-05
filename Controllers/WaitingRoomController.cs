using BankTurns.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankTurns.Controllers;

public class WaitingRoomController : Controller
{
    private readonly ITurnService _turnService;

    public WaitingRoomController(ITurnService turnService)
    {
        _turnService = turnService;
    }

    // GET /WaitingRoom
    public async Task<IActionResult> Index()
    {
        var response = await _turnService.GetQueueAsync();
        var queue    = response.Data ?? new();

        // Separar turno en atención del resto
        var inProgress = queue.Where(t => t.Status == BankTurns.Models.TurnStatus.InProgress)
                              .OrderByDescending(t => t.CalledAt)
                              .FirstOrDefault();
        var pending    = queue.Where(t => t.Status == BankTurns.Models.TurnStatus.Pending)
                              .OrderBy(t => t.CreatedAt)
                              .ToList();

        ViewBag.InProgressTurn = inProgress;
        ViewBag.PendingQueue   = pending;

        return View();
    }
}
