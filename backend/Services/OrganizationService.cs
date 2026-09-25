using backend.Data;
using backend.DTOs;
using backend.Exceptions;
using backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace backend.Services;

public class OrganizationService : IOrganizationService
{
    private readonly AppDbContext _db;

    public OrganizationService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<OrganizationResponse>> GetAllAsync(CancellationToken ct)
    {
        return await _db
            .Organizations.AsNoTracking()
            .OrderBy(o => o.Id)
            .Select(o => new OrganizationResponse(o.Id, o.Name, o.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<OrganizationResponse?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _db
            .Organizations.AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new OrganizationResponse(o.Id, o.Name, o.CreatedAt))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<OrganizationResponse> CreateAsync(
        CreateOrganizationRequest request,
        CancellationToken ct
    )
    {
        var orgExists = await _db.Organizations.AnyAsync(o => o.Name == request.Name);

        if (orgExists)
        {
            throw new ConflictException($"An organization named '{request.Name}' already exists.");
        }

        var organization = new Organization { Name = request.Name };
        _db.Organizations.Add(organization);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException e)
            when (e.InnerException
                    is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation }
            )
        {
            throw new ConflictException($"An organization named '{request.Name}' already exists.");
        }

        return new OrganizationResponse(organization.Id, organization.Name, organization.CreatedAt);
    }

    public async Task<OrganizationResponse> UpdateAsync(
        int id,
        UpdateOrganizationRequest req,
        CancellationToken ct
    )
    {
        var org = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == id, ct);

        if (org == null)
        {
            throw new NotFoundException($"Organization with id '{id}' was not found");
        }

        var orgExists = await _db.Organizations.AnyAsync(o => o.Name == req.Name && o.Id != id);
        if (orgExists)
        {
            throw new ConflictException($"An organization named '{req.Name}' already exists.");
        }

        org.Name = req.Name;
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException
                    is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation }
            )
        {
            throw new ConflictException($"An organization named '{req.Name}' already exists.");
        }
        return new OrganizationResponse(org.Id, org.Name, org.CreatedAt);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var org = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == id, ct);

        if (org == null)
        {
            throw new NotFoundException($"Organization with id '{id}' was not found");
        }

        org.IsDeleted = true;
        org.DeletedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
    }
}
