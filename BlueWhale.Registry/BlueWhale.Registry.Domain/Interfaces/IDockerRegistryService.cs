namespace BlueWhale.Registry.Domain.Interfaces;

public interface IDockerRegistryService
{
    Task<IEnumerable<RepositoryInfo>> GetRepositoriesAsync(CancellationToken cancellationToken = default);
    Task<RepositoryDetailInfo?> GetRepositoryAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<TagInfo>> GetTagsAsync(string repository, CancellationToken cancellationToken = default);
    Task<ManifestInfo?> GetManifestAsync(string repository, string reference, CancellationToken cancellationToken = default);
    Task<bool> DeleteRepositoryAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> DeleteTagAsync(string repository, string tag, CancellationToken cancellationToken = default);
}

public class RepositoryInfo
{
    public string Name { get; set; } = null!;
    public int TagCount { get; set; }
    public DateTime? LastPushed { get; set; }
}

public class RepositoryDetailInfo : RepositoryInfo
{
    public long TotalSize { get; set; }
    public IEnumerable<TagInfo> Tags { get; set; } = new List<TagInfo>();
}

public class TagInfo
{
    public string Name { get; set; } = null!;
    public long Size { get; set; }
    public DateTime? Created { get; set; }
    public string? Digest { get; set; }
}

public class ManifestInfo
{
    public string ContentType { get; set; } = null!;
    public string? Digest { get; set; }
    public string? Config { get; set; }
}
