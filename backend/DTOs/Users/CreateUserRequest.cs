using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.DTOs.Users;

public record CreateUserRequest(
    [Required] int OrganizationId,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password,
    [Required] UserRole Role
);
