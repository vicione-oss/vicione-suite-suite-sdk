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
    /// Gets or sets the subresource-integrity hash the browser checks the file against, as commonly used for CDN files.
    /// </summary>
    public string? Integrity { get; set; }

    /// <summary>
    /// Gets or sets the CORS (Cross-Origin Resource Sharing) setting for the resource, usually <c>anonymous</c>.
    /// </summary>
    public string? CrossOrigin { get; set; }

    /// <summary>
    /// Gets or sets the ID of the bundle the resource belongs to; a bundle already loaded with LoadJS is not loaded again.
    /// </summary>
    public string? Bundle { get; set; }

    /// <summary>
    /// Gets or sets whether the resource is removed when the component that loaded it is disposed (<see cref="ResourceDeclaration.Local"/>)
    /// or stays loaded (<see cref="ResourceDeclaration.Global"/>).
    /// </summary>
    public ResourceDeclaration Declaration { get; set; }

    /// <summary>
    /// Gets or sets where in the HTML document the resource belongs; <see cref="Modules.ModuleComponentBase{TComponent}"/> ignores it.
    /// </summary>
    public ResourceLocation Location { get; set; }
}
