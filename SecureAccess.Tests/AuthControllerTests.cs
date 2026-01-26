using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SecureAccess.Api.Contracts;
using SecureAccess.Api.Domain;
using SecureAccess.Api.Services;
using Xunit;

namespace SecureAccess.Tests;

public sealed class AuthControllerTests
{
    [Fact]
    public async Task Login_PublishesAuditMessage_OnSuccess()
    {
        var user = new User(Guid.NewGuid(), "test@example.com", "hash");

        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.Login("test@example.com", "pw")).Returns(user);

        var publisher = new Mock<IRabbitPublisher>();
        publisher.Setup(p => p.PublishAsync("audit.userlogins", It.IsAny<string>()))
                 .Returns(Task.CompletedTask);

        var realTokens = new TokenService(TestConfig.JwtConfig());

        var controller = new SecureAccess.Api.Controllers.AuthController(
            auth.Object,
            realTokens,
            publisher.Object
        );

        var result = await controller.Login(new LoginRequest("test@example.com", "pw"));

        publisher.Verify(p => p.PublishAsync("audit.userlogins", It.IsAny<string>()), Times.Once);
        Assert.IsType<OkObjectResult>(result.Result);
    }
}

internal static class TestConfig
{
    public static Microsoft.Extensions.Configuration.IConfiguration JwtConfig()
    {
        var settings = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "DEV_ONLY_CHANGE_ME_32_CHARS_MINIMUM_123456",
            ["Jwt:Issuer"] = "SecureAccess",
            ["Jwt:Audience"] = "SecureAccess"
        };

        return new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }
}
