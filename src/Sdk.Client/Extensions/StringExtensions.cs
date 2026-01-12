using System.Text;

namespace Sdk.Client.Extensions;

/// <summary>
/// Provides extension methods for <see cref="string"/>.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts a string from PascalCase to a hyphen-separated (kebab-case) format.
    /// </summary>
    public static string ToHyphenSeparated(this string s)
    {
        // https://stackoverflow.com/a/57517576/3936440

        if (string.IsNullOrEmpty(s))
            return s;

        var sb = new StringBuilder();

        foreach (var ch in s)
        {
            if (char.IsUpper(ch))
            {
                if (sb.Length > 0)
                    sb.Append('-');

                sb.Append(char.ToLowerInvariant(ch));
            }
            else
            {
                sb.Append(ch);
            }
        }

        return sb.ToString();
    }
}
