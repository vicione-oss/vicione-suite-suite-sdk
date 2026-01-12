using Sdk.Client.Samples;
using Sdk.Client.Samples.Shared;
using Sdk.Client.Samples.Shared.Extensions;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "wwwroot",
});

var useWebassembly = builder.Configuration.GetValue<bool>("RenderWasm");

builder.Services.AddAntiforgery();

if (useWebassembly)
{
    builder.Services
        .AddRazorComponents()
        .AddInteractiveWebAssemblyComponents();
}
else
{
    builder.Services
        .AddRazorComponents()
        .AddInteractiveServerComponents()
        .AddHubOptions(configure => configure.MaximumReceiveMessageSize = 5 * 1024 * 1024);
}

builder.Services.AddHttpClient();
builder.Services.AddLocalization();

builder.WebHost.UseStaticWebAssets();

builder.Services.AddSingleton(new RenderModeProvider(useWebassembly));
builder.Services.AddShared();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();

if (useWebassembly)
{
    app.UseWebAssemblyDebugging();
    app.MapRazorComponents<App>()
        .AddInteractiveWebAssemblyRenderMode();
}
else
{
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();
}

await app.RunAsync();
