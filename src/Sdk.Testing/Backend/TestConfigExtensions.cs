namespace Sdk.Testing.Backend;

/// <summary>
/// Provides extension methods for <see cref="TestConfig"/> to simplify adding common test configurations.
/// </summary>
public static class TestConfigExtensions
{
    extension(TestConfig config)
    {
        /// <summary>
        /// Copies <paramref name="settings"/> into the configuration, overwriting existing keys; <see langword="null"/> adds nothing.
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
        /// Sets <c>{moduleId}:Enable</c>, which enables the module unless <paramref name="disable"/> is set.
        /// </summary>
        public TestConfig AddModule(string moduleId, bool disable = false)
            => config.AddModuleInternal(moduleId, !disable);

        /// <summary>
        /// Writes each public property of <paramref name="options"/> to the module's options section, whose key is
        /// <paramref name="moduleId"/> without dots; values are stored via <see cref="object.ToString"/>.
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
