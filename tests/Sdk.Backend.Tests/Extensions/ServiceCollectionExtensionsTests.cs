using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Sdk.Backend.Extensions;
using Sdk.Modules;
using Xunit;

namespace Sdk.Backend.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{

    public sealed class AddModuleSection : ServiceCollectionExtensionsTests
    {
        private readonly ServiceCollection _serviceCollection = new();

        [Fact]
        public void Should_bind_section_options_for_module_id()
        {
            // Arrange
            var options = _serviceCollection.SetupTestOptions();

            // Act
            _serviceCollection.AddModuleSection<TestOptions>(TestConstants.TestModuleId);

            // Assert
            _serviceCollection
                .BuildServiceProvider()
                .GetRequiredService<IOptions<TestOptions>>()
                .Value.Should().BeEquivalentTo(options);
        }

        [Fact]
        public void Should_bind_section_options_for_module()
        {
            // Arrange
            var options = _serviceCollection.SetupTestOptions();

            var module = Substitute.For<IModule>();
            module.ModuleKey.Returns(new ModuleKey
            {
                ModuleId = TestConstants.TestModuleId,
                ModuleType = ModuleType.Backend
            });

            // Act
            _serviceCollection.AddModuleSection<TestOptions>(module);

            // Assert
            _serviceCollection
                .BuildServiceProvider()
                .GetRequiredService<IOptions<TestOptions>>()
                .Value.Should().BeEquivalentTo(options);
        }

        [Fact]
        public void Should_bind_default_options_if_section_is_missing()
        {
            // Arrange
            _serviceCollection.SetupTestOptions("DoesNotExist");

            // Act
            _serviceCollection.AddModuleSection<TestOptions>(TestConstants.TestModuleId);

            // Assert
            _serviceCollection
                .BuildServiceProvider()
                .GetRequiredService<IOptions<TestOptions>>()
                .Value.Should().BeEquivalentTo(new TestOptions());
        }

        [Fact]
        public void Should_validate_options_by_default()
        {
            // Arrange
            _serviceCollection.SetupInvalidTestOptions();
            _serviceCollection.AddModuleSection<TestOptions>(TestConstants.TestModuleId);

            // Act
            var action = () => _ = _serviceCollection
                .BuildServiceProvider()
                .GetRequiredService<IOptions<TestOptions>>().Value;

            // Assert
            action.Should().Throw<OptionsValidationException>();
        }

        [Fact]
        public void Should_not_validate_options_if_disabled_by_param()
        {
            // Arrange
            _serviceCollection.SetupInvalidTestOptions();
            _serviceCollection.AddModuleSection<TestOptions>(TestConstants.TestModuleId, false);

            // Act
            var action = () => _ = _serviceCollection
                .BuildServiceProvider()
                .GetRequiredService<IOptions<TestOptions>>().Value;

            // Assert
            action.Should().NotThrow<OptionsValidationException>();
        }
    }
}
