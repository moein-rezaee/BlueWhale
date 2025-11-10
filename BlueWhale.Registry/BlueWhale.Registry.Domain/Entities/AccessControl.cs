namespace BlueWhale.Registry.Domain.Entities;

public class AccessControl
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Repository { get; set; } = null!;
    public AccessLevel Level { get; set; }
    public DateTime GrantedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public enum AccessLevel
{
    None = 0,
    Pull = 1,
    Push = 2,
    Admin = 3
}
