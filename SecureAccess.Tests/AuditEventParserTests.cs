using SecureAccess.Api.Services;
using Xunit;

namespace SecureAccess.Tests;

public sealed class AuditEventParserTests
{
    [Fact]
    public void TryParseUserLoggedIn_ReturnsTrue_ForValidJson()
    {
        var json = """
        {"userId":"11111111-1111-1111-1111-111111111111","email":"a@example.com","occurredAtUtc":"2026-02-04T12:00:00Z"}
        """;

        var ok = AuditEventParser.TryParseUserLoggedIn(json, out var evt);

        Assert.True(ok);
        Assert.Equal("a@example.com", evt.Email);
    }

    [Fact]
    public void TryParseUserLoggedIn_ReturnsFalse_ForInvalidJson()
    {
        var ok = AuditEventParser.TryParseUserLoggedIn("not-json", out _);
        Assert.False(ok);
    }

    [Fact]
    public void TryParseUserLoggedIn_ReturnsFalse_ForMissingFields()
    {
        var json = """{"userId":"00000000-0000-0000-0000-000000000000","email":" ","occurredAtUtc":"2026-02-04T12:00:00Z"}""";
        var ok = AuditEventParser.TryParseUserLoggedIn(json, out _);
        Assert.False(ok);
    }
}