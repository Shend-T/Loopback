using System.Security.Cryptography;
using System.Text;
using backend.Data;
using backend.DTOs.Auth;
using backend.DTOs.Users;
using backend.Exceptions;
using backend.Models;
using backend.Services.Tokens;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace backend.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;

    public AuthService(AppDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        if (
            await _db.Organizations.AnyAsync(
                o => o.Name == request.OrganizationName && !o.IsDeleted,
                ct
            )
        )
        {
            throw new ConflictException(
                $"An organization named '{request.OrganizationName}' already exists."
            );
        }

        var user = new User
        {
            Organization = new Organization { Name = request.OrganizationName },
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12),
            Role = UserRole.Admin,
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
                $"An organization named '{request.OrganizationName}' already exists."
            );
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        _db.RefreshTokens.Add(
            new RefreshToken
            {
                UserId = user.Id,
                TokenHash = HashToken(refreshToken),
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
            }
        );
        await _db.SaveChangesAsync(ct);

        return new LoginResponse(accessToken, refreshToken);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(
            u =>
                u.Email == request.Email
                && u.Organization.Name == request.OrganizationName
                && !u.IsDeleted
                && !u.Organization.IsDeleted,
            ct
        );

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        _db.RefreshTokens.Add(
            new RefreshToken
            {
                UserId = user.Id,
                TokenHash = HashToken(refreshToken),
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
            }
        );
        await _db.SaveChangesAsync(ct);

        return new LoginResponse(accessToken, refreshToken);
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }

    public async Task<LoginResponse> RefreshAsync(RefreshRequest request, CancellationToken ct)
    {
        var hash = HashToken(request.RefreshToken);

        var stored = await _db
            .RefreshTokens.Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == hash, ct);

        if (stored is null || !stored.IsActive)
            throw new UnauthorizedException("Invalid refresh token.");

        stored.RevokedAt = DateTimeOffset.UtcNow;

        var accessToken = _tokenService.GenerateAccessToken(stored.User);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        _db.RefreshTokens.Add(
            new RefreshToken
            {
                UserId = stored.User.Id,
                TokenHash = HashToken(newRefreshToken),
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
            }
        );
        await _db.SaveChangesAsync(ct);

        return new LoginResponse(accessToken, newRefreshToken);
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken ct)
    {
        var hash = HashToken(refreshToken);

        var stored = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash, ct);

        if (stored is not null && stored.RevokedAt is null)
        {
            stored.RevokedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }
}
