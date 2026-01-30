using Microsoft.AspNetCore.Mvc;
using SecureAccess.Api.Contracts;
using SecureAccess.Api.Services;
using System.Text.Json;

namespace SecureAccess.Api.Controllers;


[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly TokenService _tokens;
    private readonly IRabbitPublisher _publisher;


    public AuthController(IAuthService auth, TokenService tokens, IRabbitPublisher publisher)
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

            var evt = new UserLoggedInEvent(user.Id, user.Email, DateTime.UtcNow);
            var json = JsonSerializer.Serialize(evt);

            await _publisher.PublishAsync("audit.userlogins", json);

            return Ok(new LoginResponse(user.Id, user.Email, token));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Invalid credentials." });
        }
    }
}
