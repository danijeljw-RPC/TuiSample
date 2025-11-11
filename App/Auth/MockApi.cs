using System;
using System.Threading.Tasks;

namespace TuiSample.App.Auth;

internal static class MockApi
{
    public static async Task<string> AuthAsync(string user, string pass)
    {
        await Task.Delay(200);
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            throw new InvalidOperationException("Missing credentials.");
        return "mock-token-" + user.ToLowerInvariant() + "-" + Guid.NewGuid().ToString("N");
    }
}
