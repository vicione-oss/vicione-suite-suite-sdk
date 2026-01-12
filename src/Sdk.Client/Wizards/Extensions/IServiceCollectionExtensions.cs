using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sdk.Client.Wizards.Builders;
using Sdk.Client.Wizards.Components;
using Sdk.Client.Wizards.Models;
using Sdk.Client.Wizards.Services;

namespace Sdk.Client.Wizards.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register services related to wizards.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds services required for a wizard associated with <typeparamref name="TContext"/> and
    /// returns a builder for further configuration.
    /// </summary>
    /// <remarks>
    /// This method registers core services, such as the <see cref="IWizardPageRegistry{TContext}"/>.
    /// </remarks>
    public static IWizardBuilder<TContext> AddWizard<TContext>(this IServiceCollection services)
    {
        services.AddWizardPageRegistry<TContext>();

        var builder = new WizardBuilder<TContext>(services);

        return builder;
    }

    private static IServiceCollection AddWizardPageRegistry<TContext>(this IServiceCollection services)
    {
        services.TryAddScoped(serviceProvider =>
        {
            var registryFactory = serviceProvider.GetRequiredService<IWizardPageRegistryFactory>();
            var registry = registryFactory.CreateWizardPageRegistry<TContext>();

            Func<IWizardPageDescriptor, IWizardPageState, IWizardPageRegistryItem> addMethodDelegate =
                registry.Add<WizardPage<IWizardPageState>, IWizardPageState>;

            var addMethodInfo = addMethodDelegate.Method.GetGenericMethodDefinition();

            var wizardPageInfos = serviceProvider.GetKeyedServices<WizardPageInfo>(typeof(TContext));
            foreach (var i in wizardPageInfos)
            {
                var descriptorType = typeof(IWizardPageDescriptor);

                if (serviceProvider.GetKeyedServices(descriptorType, i.KeyedServiceKey).FirstOrDefault() is not IWizardPageDescriptor descriptor)
                    continue;

                var state = (IWizardPageState)serviceProvider.GetRequiredKeyedService(i.StateType, i.KeyedServiceKey);

                addMethodInfo.MakeGenericMethod(i.ComponentType, i.StateType).Invoke(registry, [descriptor, state]);
            }

            return registry;
        });

        return services;
    }

    internal static IServiceCollection AddWizardPageDescriptor<TDescriptor>(this IServiceCollection services, object serviceKey)
        where TDescriptor : class, IWizardPageDescriptor
            => services.AddKeyedScoped<IWizardPageDescriptor, TDescriptor>(serviceKey);

    internal static IServiceCollection AddWizardPageState<TState>(this IServiceCollection services, object serviceKey)
        => services.AddKeyedScoped(typeof(TState), serviceKey);
}
