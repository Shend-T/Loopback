using backend.DTOs.Auth;
using backend.DTOs.Users;

namespace backend.Services.Auth;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct);

    Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken ct);
    Task<LoginResponse> RefreshAsync(RefreshRequest request, CancellationToken ct);
    Task LogoutAsync(string refreshToken, CancellationToken ct);
}
