using System.Globalization;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace Sdk.Testing.Client;

public static class TestContextExtensions
{
    /// <summary>
    /// Add ui culture, localization, logging and suite client services to service collection. 
    /// Use setup action to configure substitutes e.g. for IUiMediator
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="setup"></param>
    /// <param name="cultureName"></param>
    /// <returns></returns>
    public static TestContext SetupSuiteServices(this TestContext ctx, Action<ClientServiceConfigurator>? setup = null, string cultureName = "en-US")
    {
        ctx.SetUiCulture(cultureName);

        ctx.Services.AddLocalization();
        ctx.Services.AddLogging();
        ctx.Services.AddClientServices(setup);

        return ctx;
    }

    private static TestContext SetUiCulture(this TestContext ctx, string cultureName)
    {
        var culture = new CultureInfo(cultureName);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        return ctx;
    }
}
