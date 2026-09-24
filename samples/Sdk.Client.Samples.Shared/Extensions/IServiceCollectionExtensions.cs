using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.Samples.Shared.Services;
using Sdk.Client.Services;
using Sdk.Client.Components.Settings.Extensions;

namespace Sdk.Client.Samples.Shared.Extensions;

public static class IServiceCollectionExtensions
{
    // Registers what the sample pages need from the host. Inside the ViciOne Suite, the host provides these services;
    // a standalone app like this one has to register them itself.
    public static IServiceCollection AddShared(this IServiceCollection services)
    {
        // SDK components such as FileDropComponent load their JavaScript through IJsInterop.
        services.AddScoped<IJsInterop, JsInterop>();

        // A SettingsFieldSpinEdit needs the services for its value type registered; the settings page edits an int.
        services.AddSettingsFieldIntSpinEdit();

        return services;
    }
}
