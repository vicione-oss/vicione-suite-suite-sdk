using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Backend.Messaging;
using Sdk.Testing.Factories;

namespace Sdk.Testing.Backend;

/// <summary>
/// Provides extension methods for <see cref="IBusRegistrationConfigurator"/> to configure services for backend testing.
/// </summary>
public static class IBusRegistrationConfiguratorExtensions
{
    /// <summary>
    /// Adds the <see cref="IRoutingSlipBuilderFactory"/> to the service collection as a transient service.
    /// </summary>
    public static IBusRegistrationConfigurator AddRoutingSlipBuilderFactory(this IBusRegistrationConfigurator services)
    {
        services.AddTransient<IRoutingSlipBuilderFactory, RoutingSlipBuilderFactory>();

        return services;
    }
}
