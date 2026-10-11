using backend.Models;

namespace backend.Services.Tokens;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateInviteToken();
    string GenerateRefreshToken();

    string HashToken(string token);
}
