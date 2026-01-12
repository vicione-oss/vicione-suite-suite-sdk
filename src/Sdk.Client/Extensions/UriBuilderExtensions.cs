namespace Sdk.Client.Extensions;

public static class UriBuilderExtensions
{
    public static UriBuilder WithPath(this UriBuilder builder, string path)
    {
        builder.Path = path;
        return builder;
    }
}
