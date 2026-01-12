using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Sdk.Testing.Client;

/// <summary>
/// A factory class for creating test-specific client services.
/// </summary>
public static class ClientServiceFactory
{
    /// <summary>
    /// Creates a new <see cref="AuthenticationStateProvider"/> that provides a fixed, authenticated user principal for testing.
    /// </summary>
    public static AuthenticationStateProvider CreateAuthenticationStateProvider() => new FakeAuthenticationState();

    private sealed class FakeAuthenticationState : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(
                [
                    new Claim("Permission", "CanViewPage"), new Claim("Manager", "yes"), new Claim(ClaimTypes.Role, "Administrator"),
                    new Claim(ClaimTypes.NameIdentifier, "John")
                ], "Test"));

            var state = new AuthenticationState(principal);

            return Task.FromResult(state);
        }
    }
}
