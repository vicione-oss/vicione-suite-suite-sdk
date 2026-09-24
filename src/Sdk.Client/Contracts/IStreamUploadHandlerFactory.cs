using Sdk.Modules;

namespace Sdk.Client.Contracts;

/// <summary>
/// Creates <see cref="IStreamUploadHandler{T}"/> instances; the host implements it, and a host may replace it to customize uploads.
/// </summary>
public interface IStreamUploadHandlerFactory
{
    /// <summary>
    /// Creates a handler that uploads into the workspace of <typeparamref name="TModule"/>.
    /// </summary>
    /// <typeparam name="TMarker">Ties the handler to one upload control.</typeparam>
    /// <typeparam name="TModule">The module whose workspace receives the file.</typeparam>
    IStreamUploadHandler<TMarker> CreateStreamUploadHandler<TMarker, TModule>(StreamUploadHandlerOptions options) where TModule : IModule;

    /// <summary>
    /// Creates a handler that uploads into the workspace of the module <paramref name="moduleId"/>.
    /// </summary>
    /// <typeparam name="TMarker">Ties the handler to one upload control.</typeparam>
    IStreamUploadHandler<TMarker> CreateStreamUploadHandlerFromModuleId<TMarker>(StreamUploadHandlerOptions options, string moduleId);
}
