using BankTurns.Interfaces;
using BankTurns.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BankTurns.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TurnsController : ControllerBase
{
    private readonly ITurnService _turnService;

    public TurnsController(ITurnService turnService)
    {
        _turnService = turnService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromQuery] int userId)
    {
        var response = await _turnService.CreateAsync(userId);
        return response.Status ? Ok(response) : BadRequest(response);
    }

    [HttpGet("queue")]
    public async Task<IActionResult> GetQueue()
    {
        var response = await _turnService.GetQueueAsync();
        return Ok(response);
    }

    [HttpPost("call-next")]
    public async Task<IActionResult> CallNext([FromQuery] int advisorId)
    {
        var response = await _turnService.CallNextAsync(advisorId);
        return response.Status ? Ok(response) : BadRequest(response);
    }

    [HttpPost("{advisorId:int}/finish")]
    public async Task<IActionResult> FinishTurn(int advisorId, [FromBody] FinishTurnRequest request)
    {
        var response = await _turnService.FinishTurnAsync(advisorId, request.Comment);
        return response.Status ? Ok(response) : BadRequest(response);
    }

    [HttpGet("active/{userId:int}")]
    public async Task<IActionResult> HasActiveTurn(int userId)
    {
        var response = await _turnService.HasActiveTurnAsync(userId);
        return response.Status ? Ok(response) : NotFound(response);
    }

    [HttpGet("advisor/{advisorId:int}")]
    public async Task<IActionResult> GetAdvisorTurns(int advisorId)
    {
        var response = await _turnService.GetAdvisorTurnsAsync(advisorId);
        return Ok(response);
    }

    [HttpPost("cancel")]
    public async Task<IActionResult> Cancel([FromQuery] int userId)
    {
        var response = await _turnService.CancelTurnAsync(userId);
        return response.Status ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{turnId:int}/ticket")]
    public async Task<IActionResult> GetTicket(int turnId)
    {
        var response = await _turnService.GetTicketAsync(turnId);
        return response.Status ? Ok(response) : NotFound(response);
    }
}
