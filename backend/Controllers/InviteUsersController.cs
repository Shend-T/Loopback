using System.Security.Claims;
using backend.Auth;
using backend.DTOs.Invites;
using backend.Models;
using backend.Services.Invites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/invite")]
public class InviteUsersController : ControllerBase
{
    private readonly IInviteService _service;

    public InviteUsersController(IInviteService inviteService) => _service = inviteService;

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<InviteUserResponse>> Register(
        InviteUserRequest request,
        CancellationToken ct
    )
    {
        var caller = new CallerContext(
            Id: int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            OrganizationId: int.Parse(User.FindFirstValue("organizationId")!),
            Role: Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!)
        );

        return Ok(await _service.CreateAsync(request, caller, ct));
    }
}
