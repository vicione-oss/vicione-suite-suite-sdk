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
        /// Converts a double to a string formatted for use as an attribute value, using the invariant culture.
        /// </summary>
        public string ToAttributeValue(string addition = "")
        {
            var culture = CultureInfo.InvariantCulture;
            return string.Format(culture, "{0:F1}{1}", number, addition);
        }

        /// <summary>
        /// Converts a double to a fixed-point string representation using the current culture, with optional precision.
        /// </summary>
        public string ToFixedPointValue(int? precision = null)
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
}
