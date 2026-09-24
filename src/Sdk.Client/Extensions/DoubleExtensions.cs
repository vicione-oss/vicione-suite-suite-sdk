using System.Globalization;

namespace Sdk.Client.Extensions;

/// <summary>
/// Provides extension methods for <see cref="double"/>.
/// </summary>
public static class DoubleExtensions
{
    extension(double number)
    {
        /// <summary>
        /// Formats the number with one decimal place and the invariant culture, followed by <paramref name="addition"/>,
        /// e.g. <c>12.5deg</c> for a style attribute.
        /// </summary>
        public string ToAttributeValue(string addition = "")
        {
            var culture = CultureInfo.InvariantCulture;
            return string.Format(culture, "{0:F1}{1}", number, addition);
        }

        /// <summary>
        /// Formats the number in fixed-point notation with the current culture; <paramref name="precision"/> sets the decimal places,
        /// <see langword="null"/> uses the culture's default.
        /// </summary>
        public string ToFixedPointValue(int? precision = null)
        {
            var culture = CultureInfo.CurrentCulture;

            var format = "{0:F";
            if (precision != null)
            {
                format += precision;
            }
            format += "}";

            return string.Format(culture, format, number);
        }
    }
}
