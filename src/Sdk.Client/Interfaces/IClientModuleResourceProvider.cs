using Sdk.Client.Contracts;

namespace Sdk.Client.Services;

/// <summary>
/// Provides stylesheet and script references to be injected globally into the document
/// by the host application. Implementations are aggregated from all registered providers.
/// </summary>
public interface IClientModuleResourceProvider
{
    /// <summary>
    /// Gets the resources to include. Each resource specifies its own
    /// <see cref="ResourceLocation"/> (head or end of body).
    /// </summary>
    IEnumerable<Resource> GetResources();
}
