using backend.DTOs.Auth;
using backend.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register(
        RegisterRequest request,
        CancellationToken ct
    )
    {
        return Ok(await _authService.RegisterAsync(request, ct));
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var response = await _authService.LoginAsync(request, ct);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh(
        RefreshRequest request,
        CancellationToken ct
    )
    {
        var response = await _authService.RefreshAsync(request, ct);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshRequest req, CancellationToken ct)
    {
        await _authService.LogoutAsync(req.RefreshToken, ct);
        return NoContent();
    }
}
