namespace Sdk.Testing.Extensions;

public static class RandomExtensions
{
    /// <summary>
    /// https://stackoverflow.com/a/60847339/3936440
    /// </summary>
    public static T NextEnum<T>(this Random random)
        where T : struct, Enum
    {
        var values = Enum.GetValues<T>();

        return values[random.Next(values.Length)];
    }
}
