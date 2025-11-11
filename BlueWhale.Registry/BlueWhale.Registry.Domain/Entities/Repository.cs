using BlueWhale.Registry.Domain.Enums;

namespace BlueWhale.Registry.Domain.Entities;

/// <summary>
/// Represents a Docker registry repository configuration
/// </summary>
public class Repository
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Repository name (unique identifier for clients)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Repository display name
    /// </summary>
    public string? DisplayName { get; set; }
    
    /// <summary>
    /// Repository description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Repository type (Hosted, Proxy, or Group)
    /// </summary>
    public RepositoryType Type { get; set; } = RepositoryType.Hosted;
    
    /// <summary>
    /// Whether the repository is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// Whether the repository is public (anonymous pull allowed)
    /// </summary>
    public bool IsPublic { get; set; } = true;
    
    // --- Proxy-specific settings ---
    
    /// <summary>
    /// Upstream registry URL (for Proxy type)
    /// Example: https://registry-1.docker.io
    /// </summary>
    public string? UpstreamUrl { get; set; }
    
    /// <summary>
    /// Upstream registry username (optional, for authenticated proxies)
    /// </summary>
    public string? UpstreamUsername { get; set; }
    
    /// <summary>
    /// Upstream registry password/token (optional, for authenticated proxies)
    /// </summary>
    public string? UpstreamPassword { get; set; }
    
    /// <summary>
    /// Cache TTL in hours (for Proxy type)
    /// </summary>
    public int? CacheTtlHours { get; set; } = 24;
    
    // --- Group-specific settings ---
    
    /// <summary>
    /// Member repository IDs (for Group type)
    /// Stored as JSON array of GUIDs
    /// </summary>
    public string? MemberRepositoryIds { get; set; }
    
    // --- Storage settings ---
    
    /// <summary>
    /// Maximum storage size in bytes (null = unlimited)
    /// </summary>
    public long? MaxStorageBytes { get; set; }
    
    /// <summary>
    /// Current storage usage in bytes
    /// </summary>
    public long StorageUsedBytes { get; set; } = 0;
    
    // --- Metadata ---
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // --- Tags summary (denormalized for performance) ---
    
    public int TagCount { get; set; } = 0;
    public DateTime? LastPushedAt { get; set; }
}
