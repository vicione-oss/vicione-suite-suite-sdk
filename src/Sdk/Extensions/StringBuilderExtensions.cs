using System.Globalization;
using System.Text;

namespace Sdk.Extensions;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendFormattedLine(this StringBuilder stringBuilder, string format, params object?[] args)
    {
        stringBuilder.AppendFormat(CultureInfo.InvariantCulture, format, args);
        stringBuilder.AppendLine();
        return stringBuilder;
    }

    public static StringBuilder AppendFormattedLine(this StringBuilder stringBuilder, IFormatProvider provider, string format, params object?[] args)
    {
        stringBuilder.AppendFormat(provider, format, args);
        stringBuilder.AppendLine();
        return stringBuilder;
    }
}
