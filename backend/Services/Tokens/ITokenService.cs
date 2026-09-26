using backend.Models;

namespace backend.Services.Tokens;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
