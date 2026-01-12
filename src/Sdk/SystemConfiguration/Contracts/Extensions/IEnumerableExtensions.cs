namespace Sdk.SystemConfiguration.Contracts.Extensions;

/// <summary>
/// Provides extensions for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Gets a hash code for <paramref name="sequence"/>.
    /// </summary>
    public static int GetSequenceHashCode<T>(this IEnumerable<T> sequence)
    {
        var hashCode = new HashCode();

        foreach (var item in sequence)
        {
            hashCode.Add(item?.GetHashCode() ?? 0);
        }

        return hashCode.ToHashCode();
    }

}
