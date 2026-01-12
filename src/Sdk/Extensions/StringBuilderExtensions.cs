using System.Globalization;
using System.Text;

namespace Sdk.Extensions;

/// <summary>
/// Provides extension methods for <see cref="StringBuilder"/>.
/// </summary>
public static class StringBuilderExtensions
{
    /// <summary>
    /// Appends a line to the <see cref="StringBuilder"/> after formatting it with the specified arguments, using the invariant culture.
    /// </summary>
    public static StringBuilder AppendFormattedLine(this StringBuilder stringBuilder, string format, params object?[] args)
    {
        stringBuilder.AppendFormat(CultureInfo.InvariantCulture, format, args);
        stringBuilder.AppendLine();
        return stringBuilder;
    }

    /// <summary>
    /// Appends a line to the <see cref="StringBuilder"/> after formatting it with the specified arguments, using the specified format provider.
    /// </summary>
    public static StringBuilder AppendFormattedLine(this StringBuilder stringBuilder, IFormatProvider provider, string format, params object?[] args)
    {
        stringBuilder.AppendFormat(provider, format, args);
        stringBuilder.AppendLine();
        return stringBuilder;
    }
}
