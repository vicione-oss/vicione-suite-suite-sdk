using System.Globalization;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.Infrastructure;

namespace Sdk.Testing.Client;

/// <summary>
/// Provides extension methods for bUnit's <see cref="BunitContext"/> to simplify the setup of test environments.
/// </summary>
public static class BunitContextExtensions
{
    /// <summary>
    /// Adds UI culture, localization, logging and Suite client services to the service collection owned by <paramref name="ctx"/>.
    /// </summary>
    /// <remarks>
    /// Use <paramref name="setup"/> to configure substitutes, e.g. for <see cref="IUiMediator"/>.
    /// </remarks>
    public static BunitContext SetupSuiteServices(this BunitContext ctx, Action<ClientServiceConfigurator>? setup = null, string cultureName = "en-US")
    {
        ctx.SetUiCulture(cultureName);

        ctx.Services.AddLocalization();
        ctx.Services.AddLogging();
        ctx.Services.AddClientServices(setup);

        return ctx;
    }

    private static BunitContext SetUiCulture(this BunitContext ctx, string cultureName)
    {
        var culture = new CultureInfo(cultureName);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        return ctx;
    }
}
