using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(IUserService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var user = await service.GetUserByIdAsync(id);
        if (user is null)
            return NotFound();
        return Ok(user);
    }

    [HttpGet]
    // [Authorize]
    public async Task<IActionResult> GetAll()
        => Ok(await service.GetAllUsersAsync());
}

public record CreateUserRequest(string Username, string Email);
