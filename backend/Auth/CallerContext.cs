using backend.Models;

namespace backend.Auth;

public record CallerContext(int Id, int OrganizationId, UserRole Role);
