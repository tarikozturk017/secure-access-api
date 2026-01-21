using Microsoft.Extensions.Configuration;
using SecureAccess.Api.Domain;
using SecureAccess.Api.Services;
using Xunit;

namespace SecureAccess.Tests;

public sealed class TokenServiceTests
{
    [Fact]
    public void CreateToken_Returns_JwtLikeString()
    {
        var settings = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "DEV_ONLY_CHANGE_ME_32_CHARS_MINIMUM_123456",
            ["Jwt:Issuer"] = "SecureAccess",
            ["Jwt:Audience"] = "SecureAccess"
        };

        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var tokens = new TokenService(config);
        var user = new User(Guid.NewGuid(), "test@example.com", "hash");

        var token = tokens.CreateToken(user);

        // Assert: JWT format is 3 dot-separated parts
        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Equal(3, token.Split('.').Length);
    }
}