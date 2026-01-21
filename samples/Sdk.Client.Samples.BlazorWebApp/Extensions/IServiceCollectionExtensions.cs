using Sdk.Client.Samples.BlazorWebApp.Components;
using Sdk.Client.Samples.Shared.Components;

namespace Sdk.Client.Samples.BlazorWebApp.Extensions;

internal static class IServiceCollectionExtensions
{
    public static WebApplication UseRenderMode(this WebApplication app, bool useWebAssembly)
    {
        app.MapStaticAssets();

        var endpointConventionBuilder = app.MapRazorComponents<App>()
            .AddAdditionalAssemblies([typeof(Routes).Assembly]);

        if (useWebAssembly)
            endpointConventionBuilder.AddInteractiveWebAssemblyRenderMode();
        else
            endpointConventionBuilder.AddInteractiveServerRenderMode();

        return app;
    }
}
