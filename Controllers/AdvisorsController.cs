using BankTurns.Interfaces;
using BankTurns.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BankTurns.Controllers;

public class AdvisorsController : Controller
{
    private readonly IAdvisorService _advisorService;

    public AdvisorsController(IAdvisorService advisorService)
    {
        _advisorService = advisorService;
    }

    // POST /Advisors/Create
    [HttpPost]
    public async Task<IActionResult> Create(CreateAdvisorRequest request)
    {
        var response = await _advisorService.CreateAsync(request.Name, request.Email, request.Password);

        if (!response.Status)
        {
            TempData["Error"] = response.Message;
            return RedirectToAction("Index", "AdvisorPanel");
        }

        TempData["Success"] = $"Asesor {response.Data?.Name} creado exitosamente.";
        return RedirectToAction("Index", "AdvisorPanel");
    }

    // POST /Advisors/Login
    [HttpPost]
    public async Task<IActionResult> Login(LoginAdvisorRequest request)
    {
        var response = await _advisorService.LoginAsync(request.Email, request.Password);

        if (!response.Status || response.Data == null)
        {
            TempData["Error"] = response.Message;
            return RedirectToAction("Index", "AdvisorPanel");
        }

        HttpContext.Session.SetInt32("AdvisorId",   response.Data.Id);
        HttpContext.Session.SetString("AdvisorName", response.Data.Name);

        return RedirectToAction("Panel", "AdvisorPanel");
    }

    // POST /Advisors/Logout
    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "AdvisorPanel");
    }
}
