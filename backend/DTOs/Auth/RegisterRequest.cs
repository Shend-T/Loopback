using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Auth;

public record RegisterRequest(
    [Required, MinLength(3), MaxLength(100)] string OrganizationName,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password
);
