using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.DTOs.Invites;

public record InviteUserRequest(
    [Required, EmailAddress] string Email,
    UserRole Role = UserRole.Member
);
