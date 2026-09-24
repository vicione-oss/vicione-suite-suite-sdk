using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Sdk.Testing.Backend;

/// <summary>
/// Provides extension methods for configuring a fake security context for testing purposes.
/// </summary>
public static class TestSecurityConfiguration
{
    /// <summary>
    /// Replaces authentication and authorization so every request is authenticated as an Administrator and every policy passes.
    /// </summary>
    public static IServiceCollection AddTestSecurity(this IServiceCollection services)
    {
        services
            .AddSingleton<IPolicyEvaluator>(new FakePolicyEvaluator())
            .AddSingleton<IAuthorizationHandler>(new FakeAccessLevelHandler())
            .AddSingleton<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>()
            .AddSingleton(Options.Create(new AuthorizationOptions()));

        return services;
    }

    /// <summary>
    /// Succeeds the first requirement only; a policy with several requirements still fails through this handler.
    /// </summary>
    private sealed class FakeAccessLevelHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var frst = context.Requirements.FirstOrDefault();
            if (frst is not null)
                context.Succeed(frst);

            return Task.CompletedTask;
        }
    }

    private sealed class FakePolicyEvaluator : IPolicyEvaluator
    {
        public async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
        {
            const string TestScheme = "FakeScheme";
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(
                [
                    new Claim("Permission", "CanViewPage"), new Claim("Manager", "yes"),
                    new Claim(ClaimTypes.Role, "Administrator"), new Claim(ClaimTypes.NameIdentifier, "John")
                ], TestScheme));
            return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal,
                new AuthenticationProperties(), TestScheme)));
        }

        public async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy,
            AuthenticateResult authenticationResult, HttpContext context, object? resource)
            => await Task.FromResult(PolicyAuthorizationResult.Success());
    }
}
