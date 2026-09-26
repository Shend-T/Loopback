using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Auth;

public record LoginRequest([Required, EmailAddress] string Email, [Required] string Password);
