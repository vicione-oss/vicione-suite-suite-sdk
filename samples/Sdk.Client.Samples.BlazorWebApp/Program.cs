using Sdk.Client.Samples.BlazorWebApp.Extensions;
using Sdk.Client.Samples.BlazorWebApp.Services;
using Sdk.Client.Samples.Shared.Extensions;
using ViciOne.Ui.MonochromeIcons.Assets.Extensions;

var builder = WebApplication.CreateBuilder(args);

var useWebAssembly = builder.Configuration.GetValue<bool>("RenderWasm");

// Add services to the container.
var razorComponentsBuilder = builder.Services.AddRazorComponents();

if (useWebAssembly)
{
    razorComponentsBuilder.AddInteractiveWebAssemblyComponents();

    razorComponentsBuilder.Services.AddUrlBasedMonochromeIconSvgMarkupProvider();
}
else
{
    razorComponentsBuilder.AddInteractiveServerComponents();

    razorComponentsBuilder.Services.AddFileSystemBasedMonochromeIconSvgMarkupProvider<Program>();
}

builder.Services.AddSingleton(new RenderModeProvider(useWebAssembly));
builder.Services.AddShared();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    if (useWebAssembly)
        app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseRenderMode(useWebAssembly);

app.Run();
