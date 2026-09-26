using backend.Data;
using backend.DTOs.Organizations;
using backend.Exceptions;
using backend.Models;
using backend.Services.Organizations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly IOrganizationService _service;

    public OrganizationsController(IOrganizationService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrganizationResponse>>> GetAll(
        CancellationToken ct
    ) => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id}")]
    public async Task<ActionResult<OrganizationResponse>> GetById(int Id, CancellationToken ct)
    {
        return Ok(await _service.GetByIdAsync(Id, ct));
    }

    [HttpPost]
    public async Task<ActionResult<OrganizationResponse>> Create(
        CreateOrganizationRequest req,
        CancellationToken ct
    )
    {
        var org = await _service.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id = org.Id }, org);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<OrganizationResponse>> Update(
        int Id,
        UpdateOrganizationRequest req,
        CancellationToken ct
    )
    {
        return await _service.UpdateAsync(Id, req, ct);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}
