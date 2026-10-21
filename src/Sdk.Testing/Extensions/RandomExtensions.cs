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

#pragma warning disable CA5394 // Extends Random by design; the caller chooses the generator.
        return values[random.Next(values.Length)];
#pragma warning restore CA5394
    }
}
