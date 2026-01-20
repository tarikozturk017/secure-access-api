using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecureAccess.Api.Controllers;

[ApiController]
[Route("me")]
public sealed class MeController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public ActionResult<object> Get()
    {
        return Ok(new
        {
            user = User.Identity?.Name,
            claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }
}
