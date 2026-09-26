using backend.Auth;
using backend.DTOs.Users;

namespace backend.Services.Users;

public interface IUserService
{
    Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken ct);
    Task<UserResponse?> GetByIdAsync(int id, CancellationToken ct);
    Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken ct);
    Task<UserResponse> UpdateAsync(
        int id,
        UpdateUserRequest request,
        CallerContext caller,
        CancellationToken ct
    );
}
