namespace BlueWhale.Registry.Domain.Entities;

public class ActivityLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; } = null!;
    public string ResourceType { get; set; } = null!;
    public string? ResourceId { get; set; }
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; }
    public bool Success { get; set; }
    
    public User? User { get; set; }
}
