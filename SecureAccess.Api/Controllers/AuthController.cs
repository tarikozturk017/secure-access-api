using Microsoft.AspNetCore.Mvc;
using SecureAccess.Api.Contracts;
using SecureAccess.Api.Services;

namespace SecureAccess.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public ActionResult<AuthResponse> Register(RegisterRequest req)
    {
        try
        {
            var user = _auth.Register(req.Email, req.Password);
            return Ok(new AuthResponse(user.Id, user.Email));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public ActionResult<AuthResponse> Login(LoginRequest req)
    {
        try
        {
            var user = _auth.Login(req.Email, req.Password);
            return Ok(new AuthResponse(user.Id, user.Email));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Invalid credentials." });
        }
    }
}
