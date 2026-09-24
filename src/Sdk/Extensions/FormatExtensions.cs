namespace Sdk.Extensions;

/// <summary>
/// Provides extension methods for formatting values.
/// </summary>
public static class FormatExtensions
{
    private const int Factor = 1024;

    /// <summary>
    /// Converts a limit in megabytes (1024 × 1024 bytes) to bytes; despite the name, the conversion goes from megabytes to bytes.
    /// </summary>
    [Obsolete("Unused, and the name states the conversion backwards. This method will be removed in the next major version.")]
    public static long CalculateBytesToMb(this int limit) => (long)Factor * Factor * limit;
}
