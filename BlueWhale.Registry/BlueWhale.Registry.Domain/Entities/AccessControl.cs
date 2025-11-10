namespace BlueWhale.Registry.Domain.Entities;

public class AccessControl
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Repository { get; set; }
    public AccessLevel AccessLevel { get; set; } = AccessLevel.Pull;
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    public virtual User? User { get; set; }
}

public enum AccessLevel
{
    None = 0,
    Pull = 1,
    Push = 2,
    Admin = 3
}
