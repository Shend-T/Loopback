using backend.Data;
using backend.DTOs;
using backend.Exceptions;
using backend.Models;
using backend.Services;
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
        try
        {
            var org = await _service.CreateAsync(req, ct);
            return CreatedAtAction(nameof(GetById), new { id = org.Id }, org);
        }
        catch (DuplicateNameException e)
        {
            return Conflict(e.Message);
        }
    }
}
