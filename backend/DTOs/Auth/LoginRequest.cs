using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Auth;

public record LoginRequest(
    [Required] string OrganizationName,
    [Required, EmailAddress] string Email,
    [Required] string Password
);
