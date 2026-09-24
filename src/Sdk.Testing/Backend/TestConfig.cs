using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Sdk.Testing.Backend;

/// <summary>
/// A helper class for creating test-specific <see cref="IConfiguration"/> instances.
/// </summary>
public class TestConfig
{
    private const string InstanceTypeKey = "Instance:Type";
    private const string HomeDirectoryKey = "Instance:HomeDirectory";
    private const string CacheDirectoryKey = "Instance:CacheDirectory";
    private const string BackupDirectoryKey = "Instance:BackupDirectory";
    private const string LogPathKey = "Logging:LogPath";
    private const string DefaultLoggingKey = "Logging:LogLevel:Default";
    private const string MessageBusInMemoryKey = "MessageBus:UseInMemoryBus";
    private const string HostManagementMockClientKey = "HostManagement:UseMockClient";

    /// <summary>
    /// Gets the settings, keyed by colon-separated configuration path; later builds see changes made here.
    /// </summary>
    public Dictionary<string, string?> CurrentSettings { get; } = [];

    /// <summary>
    /// Creates a configuration, by default pre-filled for a standalone instance with an in-memory bus,
    /// a mocked host-management client and <c>Error</c> log level.
    /// </summary>
    /// <param name="addDefaults"><see langword="false"/> starts from an empty configuration.</param>
    public TestConfig(bool addDefaults = true)
    {
        if (addDefaults)
            AddDefaultSettings();
    }

    /// <summary>
    /// Builds an <see cref="IConfiguration"/> instance from the current settings.
    /// </summary>
    public IConfiguration BuildConfiguration()
        => GetInMemoryConfigurationBuilder().Build();

    /// <summary>
    /// Returns a builder seeded with a snapshot of the current settings, for adding further sources.
    /// </summary>
    public IConfigurationBuilder GetInMemoryConfigurationBuilder()
        => new ConfigurationBuilder().AddInMemoryCollection(CurrentSettings);

    /// <summary>
    /// Sets or updates a specific configuration value.
    /// </summary>
    public TestConfig SetSetting(string key, string? value)
    {
        CurrentSettings[key] = value;
        return this;
    }

    /// <summary>
    /// Sets <c>Instance:HomeDirectory</c>, the application's home directory.
    /// </summary>
    public TestConfig SetAppDirectory(string? value)
    {
        CurrentSettings[HomeDirectoryKey] = value;
        return this;
    }

    /// <summary>
    /// Sets <c>Instance:CacheDirectory</c>, the application's cache directory.
    /// </summary>
    public TestConfig SetCacheDirectory(string? value)
    {
        CurrentSettings[CacheDirectoryKey] = value;
        return this;
    }

    /// <summary>
    /// Sets <c>Instance:BackupDirectory</c>, the application's backup directory.
    /// </summary>
    public TestConfig SetBackupDirectory(string? value)
    {
        CurrentSettings[BackupDirectoryKey] = value;
        return this;
    }

    private void AddDefaultSettings()
    {
        CurrentSettings.TryAdd(InstanceTypeKey, "Standalone");
        CurrentSettings.TryAdd(HomeDirectoryKey, "AppData");
        CurrentSettings.TryAdd(CacheDirectoryKey, "Cache");
        CurrentSettings.TryAdd(BackupDirectoryKey, "Backup");
        CurrentSettings.TryAdd(LogPathKey, Directory.GetCurrentDirectory());
        CurrentSettings.TryAdd(DefaultLoggingKey, nameof(LogLevel.Error));
        CurrentSettings.TryAdd(MessageBusInMemoryKey, "true");
        CurrentSettings.TryAdd(HostManagementMockClientKey, "true");
    }
}
