using backend.DTOs.Users;
using backend.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll(CancellationToken ct) =>
        Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetById(int id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(
        CreateUserRequest req,
        CancellationToken ct
    )
    {
        var user = await _service.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }
}
