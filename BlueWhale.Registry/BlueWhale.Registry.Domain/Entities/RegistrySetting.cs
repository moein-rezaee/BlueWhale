namespace BlueWhale.Registry.Domain.Entities;

public class RegistrySetting
{
    public Guid Id { get; set; }
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public SettingCategory Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum SettingCategory
{
    Registry = 0,
    Security = 1,
    Storage = 2,
    Cache = 3,
    Proxy = 4
}
