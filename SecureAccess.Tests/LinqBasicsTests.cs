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
}
