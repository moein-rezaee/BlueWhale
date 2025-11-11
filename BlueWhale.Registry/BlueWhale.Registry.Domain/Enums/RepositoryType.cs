namespace BlueWhale.Registry.Domain.Enums;

/// <summary>
/// Defines the type of repository for Docker image storage
/// </summary>
public enum RepositoryType
{
    /// <summary>
    /// Hosted repository: stores Docker images directly in this registry
    /// This is the default type for push operations
    /// </summary>
    Hosted = 1,
    
    /// <summary>
    /// Proxy repository: caches images from an upstream registry (e.g., Docker Hub)
    /// Images are fetched on-demand and cached locally
    /// </summary>
    Proxy = 2,
    
    /// <summary>
    /// Group repository: aggregates multiple hosted and/or proxy repositories
    /// Provides a single endpoint to access images from multiple sources
    /// </summary>
    Group = 3
}
