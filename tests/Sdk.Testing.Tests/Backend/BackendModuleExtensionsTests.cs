using AwesomeAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Backend.Modules;
using Sdk.Instance;
using Sdk.Testing.Backend;
using Xunit;

namespace Sdk.Testing.Tests.Backend;

public sealed class BackendModuleExtensionsTests
{
    public sealed class TestModuleInitialization
    {
        [Fact]
        public void Should_setup_request_response()
        {
            // Arrange
            var module = new LocalTestBackendModule();

            // Act
            module.TestModuleInitialization();

            // Assert
            module.ConfigureServicesCalled.Should().BeTrue();
            module.ConfigureMessageBusCalled.Should().BeTrue();
        }
    }

    public sealed class LocalTestBackendModule : BackendModule
    {
        public bool ConfigureServicesCalled { get; private set; }
        public bool ConfigureMessageBusCalled { get; private set; }

        public override void ConfigureServices(IServiceCollection services, IConfiguration config, IMvcBuilder builder)
            => ConfigureServicesCalled = true;

        public override void ConfigureMessageBus(IServiceCollection busConfig, InstanceType instanceType)
            => ConfigureMessageBusCalled = true;
    }
}
