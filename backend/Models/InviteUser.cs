namespace backend.Models;

public class InviteUser
{
    public int Id { get; set; }
    public Organization Organization { get; set; } = null!;
    public int OrganizationId { get; set; }

    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    public int InvitedById { get; set; }
    public User InvitedBy { get; set; } = null!;
    public string TokenHash { get; set; } = string.Empty;

    public DateTimeOffset ExpiryDate { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public bool IsPending => AcceptedAt is null && ExpiryDate > DateTimeOffset.UtcNow;
}
