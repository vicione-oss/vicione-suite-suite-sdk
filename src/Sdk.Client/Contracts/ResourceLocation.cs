namespace Sdk.Client.Contracts;

/// <summary>
/// Specifies the location within the HTML document where a resource should be included.
/// </summary>
public enum ResourceLocation
{
    /// <summary>
    /// The resource should be included in the document's 'head' section.
    /// </summary>
    Head,

    /// <summary>
    /// The resource should be included at the end of the document's 'body' section.
    /// </summary>
    Body
}
