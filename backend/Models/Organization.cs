namespace backend.Models;

public class Organization
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}
