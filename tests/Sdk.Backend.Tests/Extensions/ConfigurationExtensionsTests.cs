using AwesomeAssertions;
using Sdk.Backend.Extensions;
using Sdk.Testing.Backend;
using Xunit;

namespace Sdk.Backend.Tests.Extensions;

public class ConfigurationExtensionsTests
{
    public sealed class BindModuleSection : ConfigurationExtensionsTests
    {
        [Fact]
        public void Should_bind_section_to_generic_type_for_module_id()
        {
            // Arrange
            var options = new TestOptions
            {
                BoolValue = true,
                StringValue = "Test",
                IntValue = 42,
            };
            var config = new TestConfig()
                .AddModuleWithOptions(TestConstants.TestModuleId, options)
                .BuildConfiguration();

            // Act
            var binding = config.BindModuleSection<TestOptions>(TestConstants.TestModuleId);

            // Assert
            binding.Should().BeEquivalentTo(options);
        }

        [Fact]
        public void Should_use_default_value_for_missing_section()
        {
            // Arrange
            var config = new TestConfig()
                .BuildConfiguration();

            // Act
            var binding = config.BindModuleSection<TestOptions>(TestConstants.TestModuleId);

            // Assert
            binding.Should().BeEquivalentTo(new TestOptions());
        }
    }

    public sealed class BindSection : ConfigurationExtensionsTests
    {
        private const string TestSectionKey = "ConfigSection";
        private readonly TestOptions _options = new()
        {
            BoolValue = true,
            StringValue = "Test",
            IntValue = 42,
        };

        [Fact]
        public void Should_bind_section_to_generic_type_for_section_key()
        {
            // Arrange
            var config = new TestConfig()
                .AddModuleWithOptions(TestSectionKey, _options)
                .BuildConfiguration();

            // Act
            var binding = config.BindSection<TestOptions>(TestSectionKey);

            // Assert
            binding.Should().BeEquivalentTo(_options);
        }

        [Fact]
        public void Should_use_default_value_for_missing_section()
        {
            // Arrange
            var config = new TestConfig()
                .BuildConfiguration();

            // Act
            var binding = config.BindSection<TestOptions>(TestSectionKey);

            // Assert
            binding.Should().BeEquivalentTo(new TestOptions());
        }

        [Fact]
        public void Should_use_given_default_value_for_missing_section()
        {
            // Arrange
            var config = new TestConfig()
                .BuildConfiguration();

            // Act
            var binding = config.BindSection(TestSectionKey, _options);

            // Assert
            binding.Should().BeEquivalentTo(_options);
        }

        [Fact]
        public void Should_keep_default_values_the_section_does_not_set()
        {
            // Arrange
            var config = new TestConfig(addDefaults: false)
                .SetSetting($"{TestSectionKey}:IntValue", "7")
                .BuildConfiguration();

            // Act
            var binding = config.BindSection(TestSectionKey, _options);

            // Assert
            binding.IntValue.Should().Be(7);
            binding.BoolValue.Should().BeTrue();
            binding.StringValue.Should().Be("Test");
        }

        [Fact]
        public void Should_bind_a_scalar_section()
        {
            // Arrange
            var config = new TestConfig(addDefaults: false)
                .SetSetting(TestSectionKey, "5")
                .BuildConfiguration();

            // Act
            var binding = config.BindSection(TestSectionKey, 1);

            // Assert
            binding.Should().Be(5);
        }
    }
}
