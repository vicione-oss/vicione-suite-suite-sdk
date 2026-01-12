using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Sdk.Testing.Client;

public static class ClientServiceFactory
{
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
