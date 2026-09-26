using backend.Models;

namespace backend.DTOs.Users;

public record UpdateUserRequest(string Email, string Password, UserRole Role);
