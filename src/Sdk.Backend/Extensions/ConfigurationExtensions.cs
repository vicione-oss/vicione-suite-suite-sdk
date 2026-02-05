using Microsoft.Extensions.Configuration;

namespace Sdk.Backend.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IConfiguration"/> to simplify binding strongly-typed options
/// from configuration sections, with support for module-specific sections and default values.
/// </summary>
public static class ConfigurationExtensions
{
    extension(IConfiguration config)
    {
        /// <summary>
        /// Binds a configuration section for a given module ID to a new instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type to bind the configuration values to. Must have a public parameterless constructor.
        /// </typeparam>
        /// <param name="moduleId">The module identifier used to locate the configuration section. Dots are removed before lookup.</param>
        /// <returns>
        /// A new instance of <typeparamref name="T"/> with values populated from the configuration section
        /// or default values if the section is not present.
        /// </returns>
        public T BindModuleSection<T>(string moduleId)
            where T : new()
            => config.BindSection(moduleId.Replace(".", "", StringComparison.Ordinal), new T());

        /// <summary>
        /// Binds a named configuration section to a new instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type to bind the configuration values to. Must have a public parameterless constructor.
        /// </typeparam>
        /// <param name="key">The key identifying the configuration section to bind.</param>
        /// <returns>
        /// A new instance of <typeparamref name="T"/> with values populated from the configuration section
        /// or default values if the section is not present.
        /// </returns>
        public T BindSection<T>(string key)
            where T : new()
            => config.BindSection(key, new T());

        /// <summary>
        /// Binds a named configuration section to an existing default instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type to bind the configuration values to.
        /// </typeparam>
        /// <param name="key">The key identifying the configuration section to bind.</param>
        /// <param name="defaultValue">An existing instance of <typeparamref name="T"/> that provides default values to populate.</param>
        /// <returns>
        /// The <paramref name="defaultValue"/> instance with values populated from the configuration section.
        /// If the section does not exist or is empty, the given <paramref name="defaultValue"/> is returned unchanged.
        /// </returns>
        /// <remarks>
        /// This method first checks if the section's <see cref="IConfigurationSection.Value"/> is null:
        /// <list type="bullet">
        /// <item><description>If null, it attempts to get a bound instance of <typeparamref name="T"/> and returns it, or returns <paramref name="defaultValue"/> if binding yields null.</description></item>
        /// <item><description>Otherwise, it binds the configuration values into <paramref name="defaultValue"/> and returns it.</description></item>
        /// </list>
        /// </remarks>
        public T BindSection<T>(string key, T defaultValue)
        {
            var section = config.GetSection(key);
            if (section.Value is null)
            {
                var options = section.Get<T>();
                return options ?? defaultValue;
            }

            section.Bind(defaultValue);
            return defaultValue;
        }
    }
}
