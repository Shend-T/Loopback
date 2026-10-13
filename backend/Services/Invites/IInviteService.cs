using backend.Auth;
using backend.DTOs.Invites;

namespace backend.Services.Invites;

public interface IInviteService
{
    Task<InviteUserResponse> CreateAsync(
        InviteUserRequest request,
        CallerContext caller,
        CancellationToken ct
    );

    Task<InviteInfoResponse> GetInfoAsync(InviteTokenRequest request, CancellationToken ct);
}
