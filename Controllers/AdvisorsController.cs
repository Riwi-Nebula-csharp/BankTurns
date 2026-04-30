using BankTurns.Interfaces;
using BankTurns.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BankTurns.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdvisorsController : ControllerBase
{
    private readonly IAdvisorService _advisorService;

    public AdvisorsController(IAdvisorService advisorService)
    {
        _advisorService = advisorService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAdvisorRequest request)
    {
        var response = await _advisorService.CreateAsync(request.Name, request.Email, request.Password);
        return response.Status ? Ok(response) : BadRequest(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginAdvisorRequest request)
    {
        var response = await _advisorService.LoginAsync(request.Email, request.Password);
        return response.Status ? Ok(response) : Unauthorized(response);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var response = await _advisorService.GetAllActiveAsync();
        return Ok(response);
    }

    [HttpPatch("{advisorId:int}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(int advisorId)
    {
        var response = await _advisorService.ToggleStatusAsync(advisorId);
        return response.Status ? Ok(response) : NotFound(response);
    }
}
