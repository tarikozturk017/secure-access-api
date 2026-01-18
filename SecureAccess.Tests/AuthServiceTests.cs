using SecureAccess.Api.Repositories;
using SecureAccess.Api.Services;
using Xunit;

namespace SecureAccess.Tests;

public sealed class AuthServiceTests
{
    [Fact]
    public void Register_Succeeds_ForNewEmail()
    {
        var repo = new InMemoryUserRepository();
        var svc = new AuthService(repo);

        var user = svc.Register("test@example.com", "P@ssw0rd!");
        Assert.Equal("test@example.com", user.Email);
        Assert.False(string.IsNullOrWhiteSpace(user.PasswordHash));
    }

    [Fact]
    public void Register_Fails_ForDuplicateEmail()
    {
        var repo = new InMemoryUserRepository();
        var svc = new AuthService(repo);

        svc.Register("test@example.com", "P@ssw0rd!");
        Assert.Throws<InvalidOperationException>(() =>
            svc.Register("test@example.com", "P@ssw0rd!"));
    }

    [Fact]
    public void Login_Fails_ForWrongPassword()
    {
        var repo = new InMemoryUserRepository();
        var svc = new AuthService(repo);

        svc.Register("test@example.com", "RightPassword!");
        Assert.Throws<UnauthorizedAccessException>(() =>
            svc.Login("test@example.com", "WrongPassword!"));
    }
}
