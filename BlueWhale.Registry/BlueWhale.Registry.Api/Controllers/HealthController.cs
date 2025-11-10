using Microsoft.AspNetCore.Mvc;

namespace BlueWhale.Registry.Api.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            service = "BlueWhale.Registry"
        });
    }
}
