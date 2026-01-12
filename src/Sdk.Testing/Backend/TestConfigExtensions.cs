namespace Sdk.Testing.Backend;

/// <summary>
/// Provides extension methods for <see cref="TestConfig"/> to simplify adding common test configurations.
/// </summary>
public static class TestConfigExtensions
{
    /// <summary>
    /// Adds a dictionary of custom settings to the test configuration.
    /// </summary>
    public static TestConfig AddCustomSettings(this TestConfig config, Dictionary<string, string?>? settings)
    {
        if (settings is null)
            return config;

        foreach (var setting in settings)
            config.SetSetting(setting.Key, setting.Value);

        return config;
    }

    /// <summary>
    /// Adds a setting to enable or disable a module in the test configuration.
    /// </summary>
    public static TestConfig AddModule(this TestConfig conf, string moduleId, bool disable = false)
        => conf.AddModuleInternal(moduleId, !disable);

    /// <summary>
    /// Adds module-specific options from an anonymous or concrete object to the test configuration.
    /// </summary>
    public static TestConfig AddModuleWithOptions(this TestConfig conf, string moduleId, object? options = null)
    {
        if (options is null)
            return conf;

        var sectionKey = moduleId.Replace(".", "", StringComparison.Ordinal);

        foreach (var (key, value) in GetCustomOptionsFromObject(options))
            conf.SetSetting($"{sectionKey}:{key}", value?.ToString());

        return conf;
    }

    private static Dictionary<string, object?> GetCustomOptionsFromObject(object options)
        => options.GetType().GetProperties().ToDictionary(property => property.Name, property => property.GetValue(options));
    private static TestConfig AddModuleInternal(this TestConfig conf, string moduleId, bool enable = true)
    {
        conf.SetSetting($"{moduleId}:Enable", enable.ToString());
        return conf;
    }
}
