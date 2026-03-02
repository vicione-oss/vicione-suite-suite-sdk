using AwesomeAssertions;
using Sdk.Testing.Backend;
using Xunit;

namespace Sdk.Testing.Tests.Backend;

public sealed class TestConfigTests
{
    [Fact]
    public void Constructor_with_defaults_should_populate_default_settings()
    {
        // Act
        var config = new TestConfig();

        // Assert
        config.CurrentSettings.Should().NotBeEmpty();
        config.CurrentSettings.Should().ContainKey("Instance:Type");
        config.CurrentSettings["Instance:Type"].Should().Be("Standalone");
    }

    [Fact]
    public void Constructor_without_defaults_should_have_empty_settings()
    {
        // Act
        var config = new TestConfig(addDefaults: false);

        // Assert
        config.CurrentSettings.Should().BeEmpty();
    }

    [Fact]
    public void SetSetting_should_add_or_update_value()
    {
        // Arrange
        var config = new TestConfig(addDefaults: false);

        // Act
        var result = config.SetSetting("Custom:Key", "CustomValue");

        // Assert
        result.Should().BeSameAs(config);
        config.CurrentSettings["Custom:Key"].Should().Be("CustomValue");
    }

    [Fact]
    public void SetAppDirectory_should_set_home_directory()
    {
        // Arrange
        var config = new TestConfig(addDefaults: false);

        // Act
        config.SetAppDirectory("/custom/path");

        // Assert
        config.CurrentSettings["Instance:HomeDirectory"].Should().Be("/custom/path");
    }

    [Fact]
    public void SetCacheDirectory_should_set_cache_directory()
    {
        // Arrange
        var config = new TestConfig(addDefaults: false);

        // Act
        config.SetCacheDirectory("/cache/path");

        // Assert
        config.CurrentSettings["Instance:CacheDirectory"].Should().Be("/cache/path");
    }

    [Fact]
    public void SetBackupDirectory_should_set_backup_directory()
    {
        // Arrange
        var config = new TestConfig(addDefaults: false);

        // Act
        config.SetBackupDirectory("/backup/path");

        // Assert
        config.CurrentSettings["Instance:BackupDirectory"].Should().Be("/backup/path");
    }

    [Fact]
    public void BuildConfiguration_should_return_valid_configuration()
    {
        // Arrange
        var config = new TestConfig();

        // Act
        var configuration = config.BuildConfiguration();

        // Assert
        configuration.Should().NotBeNull();
        configuration["Instance:Type"].Should().Be("Standalone");
    }

    [Fact]
    public void BuildConfiguration_should_reflect_custom_settings()
    {
        // Arrange
        var config = new TestConfig()
            .SetSetting("Custom:Key", "Value");

        // Act
        var configuration = config.BuildConfiguration();

        // Assert
        configuration["Custom:Key"].Should().Be("Value");
    }

    [Fact]
    public void GetInMemoryConfigurationBuilder_should_return_builder()
    {
        // Arrange
        var config = new TestConfig();

        // Act
        var builder = config.GetInMemoryConfigurationBuilder();

        // Assert
        builder.Should().NotBeNull();
    }
}

