using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueWhale.Registry.Api.Controllers;

[ApiController]
[Route("v1/api/users")]
[Authorize]
public class UsersControllerSimple : ControllerBase
{
    [HttpGet]
    public IActionResult GetUsers()
    {
        return Ok(new { message = "Users stub - under development" });
    }
}
