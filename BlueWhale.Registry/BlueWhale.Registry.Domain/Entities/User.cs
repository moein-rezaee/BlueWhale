namespace BlueWhale.Registry.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public UserRole Role { get; set; } = UserRole.User;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
}

public enum UserRole
{
    Admin = 0,
    User = 1,
    ReadOnly = 2
}
