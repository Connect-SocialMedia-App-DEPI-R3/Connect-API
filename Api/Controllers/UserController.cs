using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;

namespace Api.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(IUserService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
        => Ok(await service.GetUserByIdAsync(id));

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await service.GetAllUsersAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
        => Ok(await service.CreateUserAsync(req.Username, req.Email));
}

public record CreateUserRequest(string Username, string Email);
