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
        /// Binds the section named after <paramref name="moduleId"/> without dots, e.g. <c>ViciOneSuiteOee</c>, to a new
        /// <typeparamref name="T"/>; a missing section yields <c>new T()</c>.
        /// </summary>
        public T BindModuleSection<T>(string moduleId)
            where T : new()
            => config.BindSection(moduleId.Replace(".", "", StringComparison.Ordinal), new T());

        /// <summary>
        /// Binds the section <paramref name="key"/> to a new <typeparamref name="T"/>; a missing section yields <c>new T()</c>.
        /// </summary>
        public T BindSection<T>(string key)
            where T : new()
            => config.BindSection(key, new T());

        /// <summary>
        /// Binds the section <paramref name="key"/> into <paramref name="defaultValue"/> and returns it, so values the section
        /// does not set keep their defaults; a missing section returns <paramref name="defaultValue"/> unchanged.
        /// </summary>
        /// <remarks>
        /// A scalar section, such as <c>"Key": "5"</c>, is converted to <typeparamref name="T"/> instead. So is an object section
        /// when <paramref name="defaultValue"/> is <see langword="null"/>.
        /// </remarks>
        public T BindSection<T>(string key, T defaultValue)
        {
            var section = config.GetSection(key);
            if (!section.Exists())
                return defaultValue;

            if (section.Value is not null || defaultValue is null)
                return section.Get<T>() ?? defaultValue;

            // Boxing once lets a struct receive the bound values too; for a class it is the same instance.
            object target = defaultValue;
            section.Bind(target);
            return (T)target;
        }
    }
}
