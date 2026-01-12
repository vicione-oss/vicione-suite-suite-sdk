using AwesomeAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Testing.Backend;
using Xunit;

namespace Sdk.Testing.Tests.Backend;

public class TestSecurityConfigurationTests
{
    public class AddTestSecurity
    {
        [Fact]
        public void Should_setup_authorization_services()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var serviceProvider = services
                .AddTestSecurity()
                .BuildServiceProvider();

            // Assert
            serviceProvider.GetService<IAuthorizationHandler>().Should().NotBeNull();
            serviceProvider.GetService<IAuthorizationPolicyProvider>().Should().NotBeNull();
            serviceProvider.GetService<IPolicyEvaluator>().Should().NotBeNull();
        }

        [Fact]
        public async Task Should_allow_test_authentication()
        {
            // Arrange
            var services = new ServiceCollection();
            var serviceProvider = services
                .AddTestSecurity()
                .BuildServiceProvider();

            var policyEvaluator = serviceProvider.GetRequiredService<IPolicyEvaluator>();
            var policy = new AuthorizationPolicy([new LocalRequirement()], []);

            // Act
            var result = await policyEvaluator.AuthenticateAsync(policy, new DefaultHttpContext());

            // Assert
            result.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task Should_allow_test_authorization()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var services = new ServiceCollection();
            var serviceProvider = services
                .AddTestSecurity()
                .BuildServiceProvider();

            var policyEvaluator = serviceProvider.GetRequiredService<IPolicyEvaluator>();
            var policy = new AuthorizationPolicy([new LocalRequirement()], []);
            var authentication = await policyEvaluator.AuthenticateAsync(policy, httpContext);

            // Act
            var result = await policyEvaluator.AuthorizeAsync(policy, authentication, httpContext, null);

            // Assert
            result.Succeeded.Should().BeTrue();
        }

        private sealed class LocalRequirement : IAuthorizationRequirement;
    }
}
