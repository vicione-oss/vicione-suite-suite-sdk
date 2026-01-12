namespace Sdk.Testing.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Random"/>.
/// </summary>
public static class RandomExtensions
{
    /// <summary>
    /// Returns a random member of the specified enumeration.
    /// </summary>
    public static T NextEnum<T>(this Random random)
        where T : struct, Enum
    {
        var values = Enum.GetValues<T>();

        return values[random.Next(values.Length)];
    }
}
