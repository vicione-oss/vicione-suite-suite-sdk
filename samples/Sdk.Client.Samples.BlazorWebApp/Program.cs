using Sdk.Client.Samples.BlazorWebApp.Extensions;
using Sdk.Client.Samples.BlazorWebApp.Services;
using Sdk.Client.Samples.Shared.Extensions;
using ViciOne.Ui.MonochromeIcons.Assets.Extensions;

var builder = WebApplication.CreateBuilder(args);

// The same samples run either on the server (Interactive Server) or in the browser (Interactive WebAssembly).
// The launch profiles set RenderWasm to pick one; README.md describes both modes.
var useWebAssembly = builder.Configuration.GetValue<bool>("RenderWasm");

var razorComponentsBuilder = builder.Services.AddRazorComponents();

if (useWebAssembly)
    razorComponentsBuilder.AddInteractiveWebAssemblyComponents();
else
    razorComponentsBuilder.AddInteractiveServerComponents();

// Icon providers follow where the component runs: code on the server reads the icon files from disk, code in the
// browser fetches them by URL (see the WebAssembly host). Registering the disk-based provider here in both modes covers
// every component the server renders itself.
builder.Services.AddFileSystemBasedMonochromeIconSvgMarkupProvider<Program>();

// App.razor reads the chosen render mode from this provider when it renders the page shell.
builder.Services.AddSingleton(new RenderModeProvider(useWebAssembly));

// Services the shared sample pages need, registered for the server side as well.
builder.Services.AddShared();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    if (useWebAssembly)
        app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    // The default HSTS max-age is 30 days; see https://aka.ms/aspnetcore-hsts before relying on it in production.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseRenderMode(useWebAssembly);

app.Run();
