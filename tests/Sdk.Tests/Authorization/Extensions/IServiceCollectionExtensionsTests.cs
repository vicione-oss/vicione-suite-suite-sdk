using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Authorization;
using Sdk.Authorization.Extensions;
using TestModule.Backend;

namespace Sdk.Tests.Authorization.Extensions;

public sealed class IServiceCollectionExtensionsTests
{
    public sealed class AddModuleFeature
    {
        [Fact]
        public void Should_register_feature()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddModuleFeature<TestBackendModule>("Reports", "Access to reports");

            // Assert
            using var provider = services.BuildServiceProvider();
            provider.GetServices<IModuleFeature>().Should().ContainSingle().Which.Name.Should().Be("Reports");
        }

        [Fact]
        public void Should_throw_if_feature_name_contains_underscore()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var act = () => services.AddModuleFeature<TestBackendModule>("Monthly_Reports", "Access to reports");

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*Monthly_Reports*");
        }

        [Fact]
        public void Should_throw_on_resolve_if_factory_feature_name_contains_underscore()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddModuleFeature(_ => new ModuleFeature("Module", "Monthly_Reports", "Access to reports"));
            using var provider = services.BuildServiceProvider();

            // Act
            var act = () => provider.GetServices<IModuleFeature>().ToList();

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*Monthly_Reports*");
        }
    }
}
