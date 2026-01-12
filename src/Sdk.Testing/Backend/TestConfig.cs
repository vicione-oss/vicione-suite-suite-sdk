using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Sdk.Testing.Backend;

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

    public Dictionary<string, string?> CurrentSettings { get; } = [];

    public TestConfig(bool addDefaults = true)
    {
        if (addDefaults)
            AddDefaultSettings();
    }

    public IConfiguration BuildConfiguration()
        => GetInMemoryConfigurationBuilder().Build();

    public IConfigurationBuilder GetInMemoryConfigurationBuilder()
        => new ConfigurationBuilder().AddInMemoryCollection(CurrentSettings);

    public TestConfig SetSetting(string key, string? value)
    {
        CurrentSettings[key] = value;
        return this;
    }

    public TestConfig SetAppDirectory(string? value)
    {
        CurrentSettings[HomeDirectoryKey] = value;
        return this;
    }

    public TestConfig SetCacheDirectory(string? value)
    {
        CurrentSettings[CacheDirectoryKey] = value;
        return this;
    }

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
        CurrentSettings.TryAdd(DefaultLoggingKey, LogLevel.Error.ToString());
        CurrentSettings.TryAdd(MessageBusInMemoryKey, "true");
        CurrentSettings.TryAdd(HostManagementMockClientKey, "true");
    }
}
