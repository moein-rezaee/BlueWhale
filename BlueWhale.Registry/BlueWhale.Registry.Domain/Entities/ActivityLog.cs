namespace BlueWhale.Registry.Domain.Entities;

public class ActivityLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Action { get; set; }
    public required string ResourceType { get; set; }
    public string? ResourceId { get; set; }
    public string? ResourceName { get; set; }
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool Success { get; set; } = true;
    public string? IpAddress { get; set; }
    
    public virtual User? User { get; set; }
}
