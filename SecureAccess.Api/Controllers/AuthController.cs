using Microsoft.AspNetCore.Mvc;
using SecureAccess.Api.Contracts;
using SecureAccess.Api.Services;

namespace SecureAccess.Api.Controllers;


[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly TokenService _tokens;

    public AuthController(IAuthService auth, TokenService tokens)
    {
        _auth = auth;
        _tokens = tokens;
    }

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
    public ActionResult<LoginResponse> Login(LoginRequest req)
    {
        try
        {
            var user = _auth.Login(req.Email, req.Password);
            var token = _tokens.CreateToken(user);
            return Ok(new LoginResponse(user.Id, user.Email, token));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Invalid credentials." });
        }
    }
}
