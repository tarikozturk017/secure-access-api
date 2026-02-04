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

    [Fact]
    public void Linq_All()
    {
        var audits = new List<Audit>
        {
            new("UserLoggedIn", "a@example.com", DateTime.UtcNow.AddMinutes(-10)),
            new("UserLoggedIn", "b@example.com", DateTime.UtcNow.AddMinutes(-5)),
        };

        var allAreLogins = audits.All(a => a.EventType == "UserLoggedIn");
        Assert.True(allAreLogins);
    }

    [Fact]
    public void Linq_GroupBy_CountPerEmail()
    {
        var audits = new List<Audit>
        {
            new("UserLoggedIn", "a@example.com", DateTime.UtcNow.AddMinutes(-10)),
            new("UserLoggedIn", "a@example.com", DateTime.UtcNow.AddMinutes(-5)),
            new("UserLoggedIn", "b@example.com", DateTime.UtcNow.AddMinutes(-2)),
        };

        var counts = audits
            .GroupBy(a => a.Email)
            .ToDictionary(g => g.Key, g => g.Count());

        Assert.Equal(2, counts["a@example.com"]);
        Assert.Equal(1, counts["b@example.com"]);
    }


}
