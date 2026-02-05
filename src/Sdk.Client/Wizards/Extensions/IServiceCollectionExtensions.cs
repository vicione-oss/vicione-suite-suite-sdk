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
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds services required for a wizard associated with <typeparamref name="TContext"/> and
        /// returns a builder for further configuration.
        /// </summary>
        /// <remarks>
        /// This method registers core services, such as the <see cref="IWizardPageRegistry{TContext}"/>.
        /// </remarks>
        public IWizardBuilder<TContext> AddWizard<TContext>()
        {
            services.AddWizardPageRegistry<TContext>();

            var builder = new WizardBuilder<TContext>(services);

            return builder;
        }

        private IServiceCollection AddWizardPageRegistry<TContext>()
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

        internal IServiceCollection AddWizardPageDescriptor<TDescriptor>(object serviceKey)
            where TDescriptor : class, IWizardPageDescriptor
            => services.AddKeyedScoped<IWizardPageDescriptor, TDescriptor>(serviceKey);

        internal IServiceCollection AddWizardPageState<TState>(object serviceKey)
            => services.AddKeyedScoped(typeof(TState), serviceKey);
    }
}
