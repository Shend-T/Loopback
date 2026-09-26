using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using backend.Auth;
using backend.DTOs.Users;
using backend.Models;
using backend.Services.Users;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet("{id:int}")]
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

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponse>> Update(
        int id,
        UpdateUserRequest req,
        CancellationToken ct
    )
    {
        var caller = new CallerContext(
            Id: int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            OrganizationId: int.Parse(User.FindFirstValue("organizationId")!),
            Role: Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!)
        );

        return Ok(await _service.UpdateAsync(id, req, caller, ct));
    }
}
