using System.Text;

namespace Sdk.Client.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Convert Uppercase to small case and inserting '-'
    /// </summary>
    /// <param name="s">e.g. SomeCamelCase</param>
    /// <returns>some-camel-case</returns>
    public static string ToHyphenSeparated(this string s)
    {
        // https://stackoverflow.com/a/57517576/3936440

        if (string.IsNullOrEmpty(s))
            return s;

        var sb = new StringBuilder();

        foreach (var ch in s.ToCharArray())
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
