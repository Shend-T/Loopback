using backend.Auth;
using backend.Data;
using backend.DTOs.Auth;
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

    public async Task<UserResponse> UpdateAsync(
        int id,
        UpdateUserRequest request,
        CallerContext caller,
        CancellationToken ct
    )
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user == null)
        {
            throw new NotFoundException($"User with id: {id} not found");
        }

        if (!await CanModifyUserAsync(caller, user, ct, allowSelf: true))
        {
            throw new UnauthorizedException("You are not authorized for this action");
        }

        var userExists = await _db.Users.AnyAsync(
            u => u.Id != id && u.Email == request.Email && u.OrganizationId == user.OrganizationId,
            ct
        );
        if (userExists)
        {
            throw new ConflictException(
                $"A user with email {request.Email} already exists in their organization."
            );
        }

        user.Email = request.Email;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);
        user.Role = request.Role;

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException
                    is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation }
            )
        {
            throw new ConflictException(
                $"A user with email {request.Email} already exists in their organization."
            );
        }

        return new UserResponse(user.Id, user.OrganizationId, user.Email, user.Role);
    }

    private async Task<bool> CanModifyUserAsync(
        CallerContext caller,
        User target,
        CancellationToken ct,
        bool allowSelf = false
    )
    {
        if (allowSelf && caller.Id == target.Id)
            return true;
        if (
            caller.OrganizationId == target.OrganizationId
            && (caller.Role == UserRole.Manager || caller.Role == UserRole.Admin)
        )
            return true;
        if (caller.Role == UserRole.Admin)
        {
            var callerOrg = await _db.Organizations.FirstOrDefaultAsync(
                o => o.Id == caller.OrganizationId,
                ct
            );
            if (callerOrg?.IsPlatformOrganization == true)
                return true;
        }
        return false;
    }

    public async Task DeleteAsync(int id, CallerContext caller, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            throw new NotFoundException($"User with id: {id} does not exits");
        }

        bool canDeleteUser = await CanDeleteUserAsync(user, caller, ct);

        if (canDeleteUser)
        {
            user.IsDeleted = true;
            user.DeletedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync();
        }
        else
        {
            throw new UnauthorizedException("You are not authorized to complete this action");
        }
    }

    private async Task<bool> CanDeleteUserAsync(
        User user,
        CallerContext caller,
        CancellationToken ct
    )
    {
        var isPlatformUser = await _db.Users.AnyAsync(
            u => u.Id == caller.Id && u.Organization.IsPlatformOrganization,
            ct
        );
        if (isPlatformUser)
            return true;

        if (caller.OrganizationId == user.OrganizationId && caller.Role == UserRole.Admin)
            return true;

        return false;
    }
}
