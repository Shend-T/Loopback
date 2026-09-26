using backend.Data;
using backend.DTOs.Users;
using backend.Exceptions;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace backend.Services.Users;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken ct)
    {
        return await _db
            .Users.AsNoTracking()
            .OrderBy(u => u.Id)
            .Select(u => new UserResponse(u.Id, u.OrganizationId, u.Email, u.Role))
            .ToListAsync(ct);
    }

    public async Task<UserResponse?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _db
            .Users.AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserResponse(u.Id, u.OrganizationId, u.Email, u.Role))
            .FirstOrDefaultAsync();
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken ct)
    {
        var userExists = await _db.Users.AnyAsync(
            u => u.Email == request.Email && u.OrganizationId == request.OrganizationId,
            ct
        );

        if (userExists)
        {
            throw new ConflictException(
                $"A user with email {request.Email} already exists in organization: {request.OrganizationId}."
            );
        }

        var user = new User
        {
            OrganizationId = request.OrganizationId,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12),
            Role = request.Role,
        };
        _db.Users.Add(user);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException e)
            when (e.InnerException
                    is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation }
            )
        {
            throw new ConflictException(
                $"A user with email {request.Email} already exists in organization: {request.OrganizationId}."
            );
        }

        return new UserResponse(user.Id, user.OrganizationId, user.Email, user.Role);
    }
}
