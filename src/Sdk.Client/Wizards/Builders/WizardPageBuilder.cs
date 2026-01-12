using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sdk.Client.Wizards.Components;
using Sdk.Client.Wizards.Extensions;
using Sdk.Client.Wizards.Models;
using Sdk.Client.Wizards.Services;

namespace Sdk.Client.Wizards.Builders;

internal sealed class WizardPageBuilder<TContext, TComponent, TState>(IServiceCollection services)
    : IWizardPageBuilder<TContext, TComponent, TState>
        where TComponent : class, IWizardPage<TState>
        where TState : IWizardPageState
{
    public IWizardPageBuilder<TContext, TComponent, TState> WithAutoDiscovery<TDescriptor>()
        where TDescriptor : class, IWizardPageDescriptor
    {
        var contextType = typeof(TContext);
        var componentType = typeof(TComponent);

        var wizardPageServiceKey = typeof(WizardPageServiceKey<,>).MakeGenericType(contextType, componentType);

        var wizardPageInfo = new WizardPageInfo
        {
            ComponentType = componentType,
            StateType = typeof(TState),
            DescriptorType = typeof(TDescriptor),
            KeyedServiceKey = wizardPageServiceKey
        };

        services.AddKeyedScoped(contextType, (_, __) => wizardPageInfo);

        services.AddWizardPageDescriptor<TDescriptor>(wizardPageServiceKey)
            .AddWizardPageState<TState>(wizardPageServiceKey);

        return this;
    }

    public IWizardPageBuilder<TContext, TComponent, TState> WithSaveHandler<TSaveHandler>()
        where TSaveHandler : class, IWizardPageSaveHandler<TState>
    {
        services.TryAddScoped<IWizardPageSaveHandler<TState>, TSaveHandler>();

        return this;
    }

    public IWizardPageBuilder<TContext, TComponent, TState> WithCancelHandler<TCancelHandler>()
        where TCancelHandler : class, IWizardPageCancelHandler<TState>
    {
        services.TryAddScoped<IWizardPageCancelHandler<TState>, TCancelHandler>();

        return this;
    }

    public IWizardPageBuilder<TContext, TComponent, TState> WithResetHandler<TResetHandler>()
        where TResetHandler : class, IWizardPageResetHandler<TState>
    {
        services.TryAddScoped<IWizardPageResetHandler<TState>, TResetHandler>();

        return this;
    }
}
