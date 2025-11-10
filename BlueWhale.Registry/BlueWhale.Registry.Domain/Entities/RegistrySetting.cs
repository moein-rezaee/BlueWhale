namespace BlueWhale.Registry.Domain.Entities;

public class RegistrySetting
{
    public Guid Id { get; set; }
    public required string Key { get; set; }
    public required string Value { get; set; }
    public SettingCategory Category { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public enum SettingCategory
{
    Registry = 0,
    Security = 1,
    Storage = 2,
    Cache = 3,
    Proxy = 4,
    Notification = 5
}
