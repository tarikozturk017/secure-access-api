using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SecureAccess.Api.Contracts;
using Xunit;

namespace SecureAccess.Tests;

public sealed class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_Then_Login_Works()
    {
        var email = $"test{Guid.NewGuid():N}@example.com";
        var password = "RightPassword!";

        // register
        var regRes = await _client.PostAsJsonAsync("/auth/register", new RegisterRequest(email, password));
        Assert.Equal(HttpStatusCode.OK, regRes.StatusCode);

        //login
        var loginRes = await _client.PostAsJsonAsync("/auth/login", new LoginRequest(email, password));
        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        var body = await loginRes.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body!.AccessToken));
        Assert.Equal(email, body.Email);
    }
}