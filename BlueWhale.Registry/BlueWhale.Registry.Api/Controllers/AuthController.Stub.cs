using Microsoft.AspNetCore.Mvc;

namespace BlueWhale.Registry.Api.Controllers;

[ApiController]
[Route("v1/api/auth")]
public class AuthControllerStub : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login()
    {
        return Ok(new { message = "Auth stub - under development" });
    }
}
