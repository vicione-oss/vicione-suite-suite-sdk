using System.Globalization;
using Bunit;
using Sdk.Client.Infrastructure;

namespace Sdk.Testing.Client;

/// <summary>
/// Provides extension methods for bUnit's <see cref="BunitContext"/> to simplify the setup of test environments.
/// </summary>
public static class BunitContextExtensions
{
    extension(BunitContext ctx)
    {
        /// <summary>
        /// Adds localization, logging and the Suite client substitutes to the services of <paramref name="ctx"/>, and sets the culture.
        /// </summary>
        /// <remarks>
        /// Use <paramref name="setup"/> to configure substitutes, e.g. for <see cref="IUiMediator"/>. The culture is set as the
        /// process-wide default for new threads and is not restored afterwards.
        /// </remarks>
        public BunitContext SetupSuiteServices(Action<ClientServiceConfigurator>? setup = null, string cultureName = "en-US")
        {
            ctx.SetUiCulture(cultureName);

            ctx.Services.AddLocalization();
            ctx.Services.AddLogging();
            ctx.Services.AddClientServices(setup);

            return ctx;
        }

        private BunitContext SetUiCulture(string cultureName)
        {
            var culture = new CultureInfo(cultureName);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            return ctx;
        }
    }
}
