namespace Sdk.Client.Modules;

/// <summary>
/// Specifies the Blazor hosting model under which the application is running.
/// </summary>
public enum HostingModel
{
    /// <summary>
    /// The application is running on the server using the Blazor Server hosting model.
    /// </summary>
    BlazorServer,

    /// <summary>
    /// The application is running in the client's browser using the Blazor WebAssembly hosting model.
    /// </summary>
    BlazorWasm
}
