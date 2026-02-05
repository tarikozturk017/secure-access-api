using System.Text.Json;
using SecureAccess.Api.Contracts;

namespace SecureAccess.Api.Services;

public static class AuditEventParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static bool TryParseUserLoggedIn(string payload, out UserLoggedInEvent evt)
    {
        evt = default!;

        try
        {
            var parsed = JsonSerializer.Deserialize<UserLoggedInEvent>(payload, Options);
            if (parsed is null) return false;

            if (parsed.UserId == Guid.Empty) return false;
            if (string.IsNullOrWhiteSpace(parsed.Email)) return false;

            evt = parsed;
            return true;
        }
        catch
        {
            return false;
        }
    }
}