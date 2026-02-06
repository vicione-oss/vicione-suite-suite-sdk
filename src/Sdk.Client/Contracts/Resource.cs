using System.Diagnostics;

namespace Sdk.Client.Contracts;

/// <summary>
/// Represents a web resource, such as a JavaScript or CSS file, required by a module.
/// </summary>
[DebuggerDisplay("Type = {ResourceType,nq}, Url = {Url}, Declaration = {Declaration,nq}, Bundle = {Bundle,nq}")]
public sealed class Resource
{
    /// <summary>
    /// Gets or sets an optional unique identifier for the resource.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the type of the resource, which determines how it is rendered.
    /// </summary>
    public ResourceType ResourceType { get; set; }

    /// <summary>
    /// Gets or sets the path or URL to the resource.
    /// </summary>
    public Uri? Url { get; set; }

    /// <summary>
    /// Integrity checks to increase the security of resources accessed. Especially common in CDN resources.
    /// </summary>
    public string? Integrity { get; set; }

    /// <summary>
    /// Gets or sets the CORS (Cross-Origin Resource Sharing) setting for the resource, usually `anonymous`.
    /// </summary>
    public string? CrossOrigin { get; set; }

    /// <summary>
    /// Bundle ID in case this Resource belongs to a set of Resources, which may have already been loaded using LoadJS.
    /// </summary>
    public string? Bundle { get; set; }

    /// <summary>
    /// Determines if the Resource is global, meaning that the entire solution uses it or just some modules.
    /// TODO: VERIFY that this explanation is correct.
    /// </summary>
    public ResourceDeclaration Declaration { get; set; }

    /// <summary>
    /// Gets or sets the location within the HTML document where the resource should be included.
    /// </summary>
    public ResourceLocation Location { get; set; }
}
