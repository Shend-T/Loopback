using backend.Models;

namespace backend.DTOs.Users;

public record UserResponse(int Id, int OrganizationId, string Email, UserRole Role);
