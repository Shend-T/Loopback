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
            throw new DuplicateNameException(
                $"An organization named '{request.Name}' already exists."
            );
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
            throw new DuplicateNameException(
                $"An organization named '{request.Name}' already exists."
            );
        }

        return new OrganizationResponse(organization.Id, organization.Name, organization.CreatedAt);
    }
}
