namespace Sdk.Testing.Backend;

/// <summary>
/// Provides extension methods for <see cref="TestConfig"/> to simplify adding common test configurations.
/// </summary>
public static class TestConfigExtensions
{
    extension(TestConfig config)
    {
        /// <summary>
        /// Adds a dictionary of custom settings to the test configuration.
        /// </summary>
        public TestConfig AddCustomSettings(Dictionary<string, string?>? settings)
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
        public TestConfig AddModule(string moduleId, bool disable = false)
            => config.AddModuleInternal(moduleId, !disable);

        /// <summary>
        /// Adds module-specific options from an anonymous or concrete object to the test configuration.
        /// </summary>
        public TestConfig AddModuleWithOptions(string moduleId, object? options = null)
        {
            if (options is null)
                return config;

            var sectionKey = moduleId.Replace(".", "", StringComparison.Ordinal);

            foreach (var (key, value) in GetCustomOptionsFromObject(options))
                config.SetSetting($"{sectionKey}:{key}", value?.ToString());

            return config;
        }

        private TestConfig AddModuleInternal(string moduleId, bool enable = true)
        {
            config.SetSetting($"{moduleId}:Enable", enable.ToString());
            return config;
        }
    }

    private static Dictionary<string, object?> GetCustomOptionsFromObject(object options)
        => options.GetType().GetProperties().ToDictionary(property => property.Name, property => property.GetValue(options));
}
