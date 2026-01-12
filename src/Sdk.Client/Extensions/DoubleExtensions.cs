using System.Globalization;

namespace Sdk.Client.Extensions;

public static class DoubleExtensions
{
    public static string ToAttributeValue(this double number, string addition = "")
    {
        var culture = CultureInfo.InvariantCulture;
        return string.Format(culture, "{0:F1}{1}", number, addition);
    }

    public static string ToFixedPointValue(this double number, int? precision = null)
    {
        var culture = CultureInfo.CurrentCulture;

        // build format-string with a number of decimal places, for example {0:F2}
        var format = "{0:F";
        if (precision != null)
        {
            format += precision;
        }
        format += "}";

        return string.Format(culture, format, number);
    }
}
