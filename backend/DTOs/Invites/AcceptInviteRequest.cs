using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Invites;

public record AcceptInviteRequest(
    string Token,
    [StringLength(72, MinimumLength = 8)] string Password
);
