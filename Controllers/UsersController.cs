using BankTurns.Interfaces;
using BankTurns.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BankTurns.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var response = await _userService.CreateAsync(request.Document, request.Name, request.Reason);
        return response.Status ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{document}")]
    public async Task<IActionResult> GetByDocument(string document)
    {
        var response = await _userService.GetByDocumentAsync(document);
        return response.Status ? Ok(response) : NotFound(response);
    }
}
