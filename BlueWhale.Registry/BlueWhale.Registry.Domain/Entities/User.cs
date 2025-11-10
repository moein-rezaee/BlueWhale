namespace BlueWhale.Registry.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public bool IsActive { get; set; } = true;
    public UserRole Role { get; set; } = UserRole.Admin;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public ICollection<ActivityLog> ActivityLogs { get; set; } = [];
    public ICollection<AccessControl> AccessControls { get; set; } = [];
}

public enum UserRole
{
    Admin = 0,
    Manager = 1,
    Operator = 2,
    ReadOnly = 3
}
