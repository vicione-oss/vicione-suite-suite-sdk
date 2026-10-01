namespace Sdk.Client.Extensions;

internal static class UriExtensions
{
    extension(Uri uri)
    {
        /// <summary>
        /// Gets the path the browser requests: the path of an absolute URI, or a relative URI exactly as written.
        /// </summary>
        public string RequestPath => uri.IsAbsoluteUri ? uri.AbsolutePath : uri.OriginalString;
    }
}
