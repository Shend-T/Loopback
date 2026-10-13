using backend.Models;

namespace backend.DTOs.Invites;

public record InviteInfoResponse(string Email, UserRole Role, string OrganizationName);
