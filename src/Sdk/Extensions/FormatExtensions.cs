namespace Sdk.Extensions;

public static class FormatExtensions
{
    private const int Factor = 1024;

    /// <summary>
    /// Returns the memory limit in MB
    /// </summary>
    /// <param name="limit"></param>
    /// <returns>1024 * 1024 * limit</returns>
    public static long CalculateBytesToMb(this int limit) => Factor * Factor * limit;
}
