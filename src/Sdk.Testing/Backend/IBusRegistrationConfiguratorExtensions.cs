using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Backend.Messaging;

namespace Sdk.Testing.Backend;

public static class IBusRegistrationConfiguratorExtensions
{
    public static IBusRegistrationConfigurator AddRoutingSlipBuilderFactory(this IBusRegistrationConfigurator services)
    {
        services.AddTransient<IRoutingSlipBuilderFactory, RoutingSlipBuilderFactory>();
        return services;
    }

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    /// <summary>
    /// Copy of Core.OS
    /// </summary>
    private sealed class RoutingSlipBuilderFactory : IRoutingSlipBuilderFactory
    {
        public IRoutingSlipBuilder Create(Guid trackingNumber) => new RoutingSlipBuilder(trackingNumber);
    }
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
}
