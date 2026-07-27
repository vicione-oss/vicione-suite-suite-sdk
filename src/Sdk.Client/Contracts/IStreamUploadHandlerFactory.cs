using Sdk.Modules;

namespace Sdk.Client.Contracts;

/// <summary>
/// Used to inject a customized IStreamUploadHandler type
/// </summary>
public interface IStreamUploadHandlerFactory
{
    /// <summary>
    /// Creates a new instance of IStreamUploadHandler with the specified options.
    /// </summary>
    /// <typeparam name="TMarker">marker type to match the upload to the correct handler</typeparam>
    /// <typeparam name="TModule">module to allow handler to access the correct workspace</typeparam>
    /// <param name="options">options to apply</param>
    IStreamUploadHandler<TMarker> CreateStreamUploadHandler<TMarker, TModule>(StreamUploadHandlerOptions options) where TModule : IModule;

    /// <summary>
    /// Creates a new instance of IStreamUploadHandler with the specified options.
    /// </summary>
    /// <typeparam name="TMarker">marker type to match the upload to the correct handler</typeparam>
    /// <param name="moduleId">module to allow handler to access the correct workspace</param>
    /// <param name="options">options to apply</param>
    /// <returns></returns>
    IStreamUploadHandler<TMarker> CreateStreamUploadHandlerFromModuleId<TMarker>(StreamUploadHandlerOptions options, string moduleId);
}
