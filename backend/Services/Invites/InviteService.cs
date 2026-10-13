using backend.Auth;
using backend.Data;
using backend.DTOs.Invites;
using backend.Exceptions;
using backend.Models;
using backend.Services.Tokens;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Invites;

public class InviteService : IInviteService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _config;

    public InviteService(AppDbContext db, ITokenService tokenService, IConfiguration config)
    {
        _db = db;
        _tokenService = tokenService;
        _config = config;
    }

    public async Task<InviteUserResponse> CreateAsync(
        InviteUserRequest request,
        CallerContext caller,
        CancellationToken ct
    )
    {
        if (caller.Role != UserRole.Admin)
            throw new UnauthorizedException("Only admins can invite users.");

        var userExists = await _db.Users.AnyAsync(
            u => u.Email == request.Email && u.OrganizationId == caller.OrganizationId,
            ct
        );

        if (userExists)
        {
            throw new ConflictException(
                $"A user with this email either exists in this organization, or an invite to them has been sent"
            );
        }

        var rawToken = _tokenService.GenerateInviteToken();
        var iU = await _db.InviteUsers.FirstOrDefaultAsync(
            i =>
                i.OrganizationId == caller.OrganizationId
                && i.Email == request.Email
                && i.AcceptedAt == null,
            ct
        );

        if (iU is null)
        {
            iU = new InviteUser { OrganizationId = caller.OrganizationId, Email = request.Email };
            _db.InviteUsers.Add(iU);
        }

        iU.Role = request.Role;
        iU.InvitedById = caller.Id;
        iU.TokenHash = _tokenService.HashToken(rawToken);
        iU.ExpiryDate = DateTimeOffset.UtcNow.AddDays(7);

        await _db.SaveChangesAsync(ct);

        var link = $"{_config["Frontend:BaseUrl"]}/accept-invite?token={rawToken}";

        return new InviteUserResponse(link);
    }

    public async Task<InviteInfoResponse> GetInfoAsync(
        InviteTokenRequest request,
        CancellationToken ct
    )
    {
        var hash = _tokenService.HashToken(request.Token);

        var invite = await _db
            .InviteUsers.Include(i => i.Organization)
            .FirstOrDefaultAsync(i => i.TokenHash == hash, ct);

        if (invite is null || !invite.IsPending)
            throw new NotFoundException("This invite is invalid or has expired.");

        return new InviteInfoResponse(invite.Email, invite.Organization.Name);
    }
}
