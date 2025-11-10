using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using BlueWhale.Registry.Domain.Interfaces;

namespace BlueWhale.Registry.Infrastructure.ExternalServices;

public class DockerRegistryService : IDockerRegistryService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DockerRegistryService> _logger;

    public DockerRegistryService(HttpClient httpClient, ILogger<DockerRegistryService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<RepositoryInfo>> GetRepositoriesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Fetching repositories from Docker Registry");
            
            // Docker Registry V2 API: GET /v2/_catalog
            var response = await _httpClient.GetAsync("/v2/_catalog", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch repositories: {StatusCode}", response.StatusCode);
                return Enumerable.Empty<RepositoryInfo>();
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            using var jsonDoc = JsonDocument.Parse(content);
            var repositories = new List<RepositoryInfo>();

            if (jsonDoc.RootElement.TryGetProperty("repositories", out var reposElement))
            {
                foreach (var repoName in reposElement.EnumerateArray())
                {
                    var name = repoName.GetString();
                    if (name != null)
                    {
                        var tagCount = await GetTagCountAsync(name, cancellationToken);
                        repositories.Add(new RepositoryInfo
                        {
                            Name = name,
                            TagCount = tagCount,
                            LastPushed = DateTime.UtcNow
                        });
                    }
                }
            }

            _logger.LogInformation("Successfully fetched {Count} repositories", repositories.Count);
            return repositories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching repositories");
            return Enumerable.Empty<RepositoryInfo>();
        }
    }

    public async Task<RepositoryDetailInfo?> GetRepositoryAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Fetching repository details: {Repository}", name);
            
            var tags = await GetTagsAsync(name, cancellationToken);
            var tagsArray = tags.ToList();
            var totalSize = tagsArray.Sum(t => t.Size);

            return new RepositoryDetailInfo
            {
                Name = name,
                TagCount = tagsArray.Count,
                TotalSize = totalSize,
                LastPushed = DateTime.UtcNow,
                Tags = tagsArray
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching repository: {Repository}", name);
            return null;
        }
    }

    public async Task<IEnumerable<TagInfo>> GetTagsAsync(string repository, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Fetching tags for repository: {Repository}", repository);
            
            // Docker Registry V2 API: GET /v2/<name>/tags/list
            var response = await _httpClient.GetAsync($"/v2/{repository}/tags/list", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch tags for {Repository}: {StatusCode}", repository, response.StatusCode);
                return Enumerable.Empty<TagInfo>();
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            using var jsonDoc = JsonDocument.Parse(content);
            var tags = new List<TagInfo>();

            if (jsonDoc.RootElement.TryGetProperty("tags", out var tagsElement))
            {
                foreach (var tagElement in tagsElement.EnumerateArray())
                {
                    var tag = tagElement.GetString();
                    if (tag != null)
                    {
                        var manifest = await GetManifestAsync(repository, tag, cancellationToken);
                        tags.Add(new TagInfo
                        {
                            Name = tag,
                            Size = manifest?.Digest?.Length ?? 0,
                            Created = DateTime.UtcNow,
                            Digest = manifest?.Digest
                        });
                    }
                }
            }

            _logger.LogInformation("Successfully fetched {Count} tags for {Repository}", tags.Count, repository);
            return tags;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tags: {Repository}", repository);
            return Enumerable.Empty<TagInfo>();
        }
    }

    public async Task<ManifestInfo?> GetManifestAsync(string repository, string reference, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Fetching manifest: {Repository}@{Reference}", repository, reference);
            
            // Docker Registry V2 API: GET /v2/<name>/manifests/<reference>
            var request = new HttpRequestMessage(HttpMethod.Get, $"/v2/{repository}/manifests/{reference}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.docker.distribution.manifest.v2+json"));

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch manifest for {Repository}:{Reference}: {StatusCode}", repository, reference, response.StatusCode);
                return null;
            }

            var digest = response.Headers.FirstOrDefault(h => h.Key == "Docker-Content-Digest").Value?.FirstOrDefault();
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/vnd.docker.distribution.manifest.v2+json";
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            return new ManifestInfo
            {
                ContentType = contentType,
                Digest = digest,
                Config = content
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching manifest: {Repository}@{Reference}", repository, reference);
            return null;
        }
    }

    public async Task<bool> DeleteRepositoryAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Deleting repository: {Repository}", name);
            
            var tags = await GetTagsAsync(name, cancellationToken);
            foreach (var tag in tags)
            {
                await DeleteTagAsync(name, tag.Name, cancellationToken);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting repository: {Repository}", name);
            return false;
        }
    }

    public async Task<bool> DeleteTagAsync(string repository, string tag, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Deleting tag: {Repository}:{Tag}", repository, tag);
            
            // First get the manifest digest
            var manifest = await GetManifestAsync(repository, tag, cancellationToken);
            if (manifest?.Digest == null)
            {
                _logger.LogWarning("Could not get manifest digest for {Repository}:{Tag}", repository, tag);
                return false;
            }

            // Docker Registry V2 API: DELETE /v2/<name>/manifests/<digest>
            var response = await _httpClient.DeleteAsync($"/v2/{repository}/manifests/{manifest.Digest}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to delete tag {Repository}:{Tag}: {StatusCode}", repository, tag, response.StatusCode);
                return false;
            }

            _logger.LogInformation("Successfully deleted tag {Repository}:{Tag}", repository, tag);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tag: {Repository}:{Tag}", repository, tag);
            return false;
        }
    }

    private async Task<int> GetTagCountAsync(string repository, CancellationToken cancellationToken)
    {
        try
        {
            var tags = await GetTagsAsync(repository, cancellationToken);
            return tags.Count();
        }
        catch
        {
            return 0;
        }
    }
}

