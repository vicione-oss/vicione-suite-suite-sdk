namespace Sdk.Client.Extensions;

/// <summary>
/// Provides extension methods for <see cref="UriBuilder"/>.
/// </summary>
public static class UriBuilderExtensions
{
    /// <summary>
    /// Sets the path portion of the URI in a fluent manner.
    /// </summary>
    public static UriBuilder WithPath(this UriBuilder builder, string path)
    {
        builder.Path = path;
        return builder;
    }
}
