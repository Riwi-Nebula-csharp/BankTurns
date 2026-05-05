using BankTurns.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankTurns.Controllers;

public class AdvisorPanelController : Controller
{
    private readonly ITurnService _turnService;

    public AdvisorPanelController(ITurnService turnService)
    {
        _turnService = turnService;
    }

    // GET /AdvisorPanel  — login
    public IActionResult Index()
    {
        // Si ya hay sesión, ir directo al panel
        if (HttpContext.Session.GetInt32("AdvisorId") != null)
            return RedirectToAction(nameof(Panel));

        return View();
    }

    // GET /AdvisorPanel/Panel
    public async Task<IActionResult> Panel()
    {
        var advisorId = HttpContext.Session.GetInt32("AdvisorId");
        if (advisorId == null)
            return RedirectToAction(nameof(Index));

        var advisorName = HttpContext.Session.GetString("AdvisorName");

        // Cola global de turnos (pending + in-progress)
        var queueResponse    = await _turnService.GetQueueAsync();
        // Turnos del asesor de hoy (para sección "Atendidos")
        var advisorResponse  = await _turnService.GetAdvisorTurnsAsync(advisorId.Value);
        var queueData = queueResponse.Data ?? new();
        // Turno activo actual del asesor
        var activeTurn = queueData.FirstOrDefault(t => t.AdvisorId == advisorId.Value && t.Status == BankTurns.Models.TurnStatus.InProgress);

        ViewBag.AdvisorId   = advisorId.Value;
        ViewBag.AdvisorName = advisorName;
        ViewBag.Queue       = queueData;
        ViewBag.AdvisorTurns = advisorResponse.Data ?? new();
        ViewBag.ActiveTurn  = activeTurn;

        return View();
    }

    // POST /AdvisorPanel/CallNext
    [HttpPost]
    public async Task<IActionResult> CallNext()
    {
        var advisorId = HttpContext.Session.GetInt32("AdvisorId");
        if (advisorId == null)
            return RedirectToAction(nameof(Index));

        var response = await _turnService.CallNextAsync(advisorId.Value);
        TempData[response.Status ? "Success" : "Error"] = response.Message;

        return RedirectToAction(nameof(Panel));
    }

    // POST /AdvisorPanel/FinishTurn
    [HttpPost]
    public async Task<IActionResult> FinishTurn(string? comment)
    {
        var advisorId = HttpContext.Session.GetInt32("AdvisorId");
        if (advisorId == null)
            return RedirectToAction(nameof(Index));

        var response = await _turnService.FinishTurnAsync(advisorId.Value, comment);
        TempData[response.Status ? "Success" : "Error"] = response.Message;

        return RedirectToAction(nameof(Panel));
    }
}
