namespace BlueWhale.Registry.Application.Options;

public class RegistryOptions
{
    public string RegistryUrl { get; set; } = "http://localhost:5000";
    public string? Username { get; set; }
    public string? Password { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
}

public class JwtOptions
{
    public string Secret { get; set; } = null!;
    public string Issuer { get; set; } = "bluewhale.registry";
    public string Audience { get; set; } = "bluewhale.registry";
    public int AccessTokenMinutes { get; set; } = 60;
    public int RefreshTokenDays { get; set; } = 7;
}

public class CacheOptions
{
    public string DefaultProvider { get; set; } = "Memory";
    public int DefaultTtlMinutes { get; set; } = 60;
}
