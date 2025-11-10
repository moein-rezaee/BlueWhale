using Microsoft.AspNetCore.Mvc;
using BlueWhale.Registry.Domain.Interfaces;

namespace BlueWhale.Registry.Api.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly IDockerRegistryService _registryService;
    private readonly ILogger<TagsController> _logger;

    public TagsController(IDockerRegistryService registryService, ILogger<TagsController> logger)
    {
        _registryService = registryService;
        _logger = logger;
    }

    [HttpGet("{repository}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<object>>> GetTags(string repository, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching tags for repository: {Repository}", repository);
            var tags = await _registryService.GetTagsAsync(repository, cancellationToken);
            return Ok(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tags for repository: {Repository}", repository);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to fetch tags" });
        }
    }

    [HttpGet("{repository}/{tag}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetTag(string repository, string tag, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching manifest for {Repository}:{Tag}", repository, tag);
            var manifest = await _registryService.GetManifestAsync(repository, tag, cancellationToken);
            
            if (manifest == null)
            {
                return NotFound(new { message = $"Tag '{tag}' not found in repository '{repository}'" });
            }

            return Ok(manifest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tag {Repository}:{Tag}", repository, tag);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to fetch tag" });
        }
    }

    [HttpDelete("{repository}/{tag}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteTag(string repository, string tag, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting tag {Repository}:{Tag}", repository, tag);
            var success = await _registryService.DeleteTagAsync(repository, tag, cancellationToken);
            
            if (!success)
            {
                return NotFound(new { message = $"Tag '{tag}' not found in repository '{repository}'" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tag {Repository}:{Tag}", repository, tag);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to delete tag" });
        }
    }
}
