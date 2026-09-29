namespace Sdk.Client.Components.Settings;

/// <summary>
/// Matches a file against the value of an <see href="https://html.spec.whatwg.org/#attr-input-accept">accept</see> attribute.
/// </summary>
internal static class AcceptFilter
{
    /// <summary>
    /// Returns whether the file matches at least one token of <paramref name="accept"/>, or whether the attribute has no valid token.
    /// </summary>
    /// <param name="accept">Comma-separated file extensions (<c>.png</c>), MIME types (<c>image/png</c>) or wildcards (<c>image/*</c>).</param>
    /// <param name="fileName">The name of the file, matched against extension tokens.</param>
    /// <param name="contentType">The MIME type of the file, matched against MIME type and wildcard tokens; may be empty.</param>
    public static bool Matches(string? accept, string fileName, string? contentType)
    {
        if (string.IsNullOrWhiteSpace(accept))
            return true;

        // As in the browser, tokens that are neither an extension nor a MIME type are ignored.
        var hasValidToken = false;

        foreach (var token in accept.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            if (token.Length > 1 && token[0] == '.')
            {
                hasValidToken = true;

                // EndsWith rather than Path.GetExtension, so multi-part extensions such as ".tar.gz" match as well.
                if (fileName.EndsWith(token, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            else if (token.IndexOf('/') is > 0 and var slash && slash < token.Length - 1)
            {
                hasValidToken = true;

                if (string.IsNullOrEmpty(contentType))
                    continue;

                if (token.EndsWith("/*", StringComparison.Ordinal)
                    ? contentType.AsSpan().StartsWith(token.AsSpan(0, slash + 1), StringComparison.OrdinalIgnoreCase)
                    : string.Equals(token, contentType, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }

        return !hasValidToken;
    }
}
