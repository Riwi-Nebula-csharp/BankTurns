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

        var queueResponse   = await _turnService.GetQueueAsync();
        var advisorResponse = await _turnService.GetAdvisorTurnsAsync(advisorId.Value);
        var queueData       = queueResponse.Data ?? new();
        var activeTurn      = queueData.FirstOrDefault(t =>
            t.AdvisorId == advisorId.Value &&
            t.Status    == BankTurns.Models.TurnStatus.InProgress);

        ViewBag.AdvisorId    = advisorId.Value;
        ViewBag.AdvisorName  = advisorName;
        ViewBag.Queue        = queueData;
        ViewBag.AdvisorTurns = advisorResponse.Data ?? new();
        ViewBag.ActiveTurn   = activeTurn;

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

        if (response.Status && response.Data != null)
        {
            TempData["CalledTicket"] = response.Data.Ticket;
            TempData["CalledName"]   = response.Data.User?.Name;
        }

        return RedirectToAction(nameof(Panel));
    }

    // POST /AdvisorPanel/RecallCurrent
    [HttpPost]
    public async Task<IActionResult> RecallCurrent()
    {
        var advisorId = HttpContext.Session.GetInt32("AdvisorId");
        if (advisorId == null)
            return RedirectToAction(nameof(Index));

        var response = await _turnService.RecallCurrentAsync(advisorId.Value);
        TempData[response.Status ? "Success" : "Error"] = response.Message;

        if (response.Status && response.Data != null)
        {
            TempData["CalledTicket"] = response.Data.Ticket;
            TempData["CalledName"]   = response.Data.User?.Name;
        }

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