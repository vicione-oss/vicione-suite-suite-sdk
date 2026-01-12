using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.Wizards.Components;
using Sdk.Client.Wizards.Services;

namespace Sdk.Client.Wizards.Builders;

internal sealed class WizardBuilder<TContext>(IServiceCollection services) : IWizardBuilder<TContext>
{
    public IWizardPageBuilder<TContext, TComponent, WizardPageState> WithPage<TComponent>()
        where TComponent : class, IWizardPage<WizardPageState>
            => WithPage<TComponent, WizardPageState>();

    public IWizardPageBuilder<TContext, TComponent, TState> WithPage<TComponent, TState>()
        where TComponent : class, IWizardPage<TState>
        where TState : IWizardPageState
            => new WizardPageBuilder<TContext, TComponent, TState>(services);
}
