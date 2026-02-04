using Xunit;

namespace SecureAccess.Tests;

public sealed class LinqBasicsTests
{
    private sealed record Audit(string EventType, string Email, DateTime OccurredAtUtc);

    [Fact]
    public void Linq_Where_And_Select()
    {
        var audits = new List<Audit>
        {
            new("UserLoggedIn", "a@example.com", DateTime.UtcNow.AddMinutes(-10)),
            new("UserLoggedIn", "b@example.com", DateTime.UtcNow.AddMinutes(-5)),
            new("UserRegistered", "a@example.com", DateTime.UtcNow.AddMinutes(-2)),
        };

        var loginEmails = audits
            .Where(a => a.EventType == "UserLoggedIn")
            .Select(a => a.Email)
            .ToList();

        Assert.Equal(2, loginEmails.Count);
    }

    [Fact]
    public void Linq_Any_And_FirstOrDefault()
    {
        var audits = new List<Audit>
        {
            new("UserLoggedIn", "a@example.com", DateTime.UtcNow.AddMinutes(-10)),
            new("UserRegistered", "a@example.com", DateTime.UtcNow.AddMinutes(-2)),
        };

        var hasLogin = audits.Any(a => a.EventType == "UserLoggedIn");
        Assert.True(hasLogin);

        var firstRegister = audits.FirstOrDefault(a => a.EventType == "UserRegistered");
        Assert.NotNull(firstRegister);
        Assert.Equal("a@example.com", firstRegister!.Email);
    }

}
