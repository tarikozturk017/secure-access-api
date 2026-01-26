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
    private readonly RabbitPublisher _publisher;


    public AuthController(IAuthService auth, TokenService tokens, RabbitPublisher publisher)
    {
        _auth = auth;
        _tokens = tokens;
        _publisher = publisher;
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
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest req)
    {
        try
        {
            var user = _auth.Login(req.Email, req.Password);
            var token = _tokens.CreateToken(user);

            await _publisher.PublishAsync(
                "audit.userlogins",
                $"UserLoggedIn:{user.Id}:{user.Email}:{DateTime.UtcNow:o}"
            );

            return Ok(new LoginResponse(user.Id, user.Email, token));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Invalid credentials." });
        }
    }
}
