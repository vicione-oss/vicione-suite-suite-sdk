using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Sdk.Client.Contracts;

/// <summary>
/// Resource Objects describe a JavaScript or CSS file which is needed by the Module to work. 
/// </summary>
[DebuggerDisplay("Type = {ResourceType,nq}, Url = {Url}, Declaration = {Declaration,nq}, Bundle = {Bundle,nq}")]
public sealed class Resource
{
    public string? Id { get; set; }

    /// <summary>
    /// A <see cref="ResourceType"/> so the Interop can properly create `script` or `link` tags
    /// </summary>
    public ResourceType ResourceType { get; set; }

    /// <summary>
    /// Path to the resources. 
    /// </summary>
    [SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "Can be invalid for Uri class")]
    public string? Url { get; set; }

    /// <summary>
    /// Integrity checks to increase the security of resources accessed. Especially common in CDN resources. 
    /// </summary>
    public string? Integrity { get; set; }

    /// <summary>
    /// Cross-Origin rules for this Resources. Usually `anonymous`
    /// </summary>
    public string? CrossOrigin { get; set; }

    /// <summary>
    /// Bundle ID in case this Resource belongs to a set of Resources, which may have already been loaded using LoadJS
    /// </summary>
    public string? Bundle { get; set; }

    /// <summary>
    /// Determines if the Resource is global, meaning that the entire solution uses it or just some modules.
    /// TODO: VERIFY that this explanation is correct.
    /// </summary>
    public ResourceDeclaration Declaration { get; set; }

    /// <summary>
    /// If the Resource should be included in the `head` of the HTML document or the `body`
    /// </summary>
    public ResourceLocation Location { get; set; }
}
