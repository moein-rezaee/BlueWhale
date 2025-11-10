using Microsoft.AspNetCore.Mvc;
using BlueWhale.Registry.Domain.Interfaces;

namespace BlueWhale.Registry.Api.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
public class RepositoriesController : ControllerBase
{
    private readonly IDockerRegistryService _registryService;
    private readonly ILogger<RepositoriesController> _logger;

    public RepositoriesController(IDockerRegistryService registryService, ILogger<RepositoriesController> logger)
    {
        _registryService = registryService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<object>>> GetRepositories(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching all repositories");
            var repositories = await _registryService.GetRepositoriesAsync(cancellationToken);
            return Ok(repositories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching repositories");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to fetch repositories" });
        }
    }

    [HttpGet("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetRepository(string name, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching repository: {Repository}", name);
            var repository = await _registryService.GetRepositoryAsync(name, cancellationToken);
            
            if (repository == null)
            {
                return NotFound(new { message = $"Repository '{name}' not found" });
            }

            return Ok(repository);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching repository: {Repository}", name);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to fetch repository" });
        }
    }

    [HttpDelete("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteRepository(string name, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting repository: {Repository}", name);
            var success = await _registryService.DeleteRepositoryAsync(name, cancellationToken);
            
            if (!success)
            {
                return NotFound(new { message = $"Repository '{name}' not found or deletion failed" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting repository: {Repository}", name);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to delete repository" });
        }
    }
}
