namespace backend.DTOs.Auth;

public record LoginResponse(string AccessToken, string RefreshToken);
