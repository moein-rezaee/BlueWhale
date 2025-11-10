using Microsoft.AspNetCore.Mvc;
using BlueWhale.Registry.Domain.Interfaces;

namespace BlueWhale.Registry.Api.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IDockerRegistryService _registryService;
    private readonly ILogger<StatisticsController> _logger;

    public StatisticsController(IDockerRegistryService registryService, ILogger<StatisticsController> logger)
    {
        _registryService = registryService;
        _logger = logger;
    }

    [HttpGet("summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetSummary(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching registry summary");
            
            var repositories = await _registryService.GetRepositoriesAsync(cancellationToken);
            var repoList = repositories.ToList();

            var totalSize = 0L;
            var totalTags = 0;

            foreach (var repo in repoList)
            {
                var details = await _registryService.GetRepositoryAsync(repo.Name, cancellationToken);
                if (details != null)
                {
                    totalSize += details.TotalSize;
                    totalTags += details.TagCount;
                }
            }

            return Ok(new
            {
                totalRepositories = repoList.Count,
                totalTags = totalTags,
                totalSize = totalSize,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching registry summary");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to fetch summary" });
        }
    }

    [HttpGet("repositories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetRepositoriesStats(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching repositories statistics");
            
            var repositories = await _registryService.GetRepositoriesAsync(cancellationToken);
            var stats = new List<object>();

            foreach (var repo in repositories)
            {
                var details = await _registryService.GetRepositoryAsync(repo.Name, cancellationToken);
                if (details != null)
                {
                    stats.Add(new
                    {
                        name = details.Name,
                        tagCount = details.TagCount,
                        totalSize = details.TotalSize,
                        lastPushed = details.LastPushed
                    });
                }
            }

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching repositories statistics");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to fetch statistics" });
        }
    }
}
