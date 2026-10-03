using backend.DTOs.Auth;

namespace backend.Services.Auth;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct);
    Task<LoginResponse> RefreshAsync(RefreshRequest request, CancellationToken ct);
    Task LogoutAsync(string refreshToken, CancellationToken ct);
}
