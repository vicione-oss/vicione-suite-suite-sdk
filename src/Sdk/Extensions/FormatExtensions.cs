namespace Sdk.Extensions;

/// <summary>
/// Provides extension methods for formatting values.
/// </summary>
public static class FormatExtensions
{
    private const int Factor = 1024;

    /// <summary>
    /// Calculates the total number of bytes from a given limit in megabytes (MB).
    /// </summary>
    public static long CalculateBytesToMb(this int limit) => (long)Factor * Factor * limit;
}
