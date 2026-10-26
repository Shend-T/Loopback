using System.Security.Claims;
using backend.Auth;
using backend.DTOs.Issues;
using backend.Models;
using backend.Services.Issues;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IssueController : ControllerBase
{
    private readonly IIssueService _service;

    public IssueController(IIssueService service) => _service = service;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<IssueResponse>>> GetAll(CancellationToken ct)
    {
        var caller = new CallerContext(
            Id: int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            OrganizationId: int.Parse(User.FindFirstValue("organizationId")!),
            Role: Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!)
        );

        return Ok(await _service.GetAllAsync(caller, ct));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<IssueResponse>> Create(
        IssueRequest request,
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
