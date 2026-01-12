using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Sdk.Testing.Backend;

public static class TestSecurityConfiguration
{
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
    /// fake authentication to let client access authorized controllers
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
            var testScheme = "FakeScheme";
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(
                [
                    new Claim("Permission", "CanViewPage"), new Claim("Manager", "yes"),
                    new Claim(ClaimTypes.Role, "Administrator"), new Claim(ClaimTypes.NameIdentifier, "John")
                ], testScheme));
            return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal,
                new AuthenticationProperties(), testScheme)));
        }

        public async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy,
            AuthenticateResult authenticationResult, HttpContext context, object? resource)
            => await Task.FromResult(PolicyAuthorizationResult.Success());
    }
}
