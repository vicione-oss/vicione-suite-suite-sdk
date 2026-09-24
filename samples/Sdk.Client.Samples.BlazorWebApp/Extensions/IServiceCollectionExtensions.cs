using Sdk.Client.Samples.BlazorWebApp.Components;
using Sdk.Client.Samples.Shared.Components;

namespace Sdk.Client.Samples.BlazorWebApp.Extensions;

internal static class IServiceCollectionExtensions
{
    public static WebApplication UseRenderMode(this WebApplication app, bool useWebAssembly)
    {
        app.MapStaticAssets();

        // The sample pages live in Sdk.Client.Samples.Shared, so its assembly must be added for their @page routes to be found.
        var endpointConventionBuilder = app.MapRazorComponents<App>()
            .AddAdditionalAssemblies([typeof(Routes).Assembly]);

        // Only the render mode that Program.cs registered services for may be enabled here.
        if (useWebAssembly)
            endpointConventionBuilder.AddInteractiveWebAssemblyRenderMode();
        else
            endpointConventionBuilder.AddInteractiveServerRenderMode();

        return app;
    }
}
