namespace Sdk.SystemConfiguration.Contracts.Extensions;

/// <summary>
/// Provides extensions for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Gets a sequence hash code
    /// </summary>
    /// <typeparam name="T">The type of objects in the sequence.</typeparam>
    /// <param name="sequence">The sequence.</param>
    /// <returns>A hash code for the current sequence.</returns>
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
