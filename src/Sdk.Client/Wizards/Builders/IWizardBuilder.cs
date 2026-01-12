using Sdk.Client.Wizards.Components;
using Sdk.Client.Wizards.Services;

namespace Sdk.Client.Wizards.Builders;

/// <summary>
/// Fluent builder to configure a wizard and its pages.
/// </summary>
/// <typeparam name="TContext">The context type associated with the wizard.</typeparam>
public interface IWizardBuilder<TContext>
{
    /// <summary>
    /// Adds a page to the wizard implemented by a specific component and state type.
    /// </summary>
    /// <returns>
    /// Builder for a wizard associated with <typeparamref name="TContext"/> that should contain
    /// a page implemented by <typeparamref name="TComponent"/> based on <typeparamref name="TState"/>.
    /// </returns>
    IWizardPageBuilder<TContext, TComponent, TState> WithPage<TComponent, TState>()
        where TComponent : class, IWizardPage<TState>
        where TState : IWizardPageState;

    /// <summary>
    /// Adds a page to the wizard implemented by a specific component and the default state type.
    /// </summary>
    /// <returns>
    /// Builder for a wizard associated with <typeparamref name="TContext"/> that should contain
    /// a page implemented by <typeparamref name="TComponent"/> based on <see cref="WizardPageState"/>.
    /// </returns>
    IWizardPageBuilder<TContext, TComponent, WizardPageState> WithPage<TComponent>()
        where TComponent : class, IWizardPage<WizardPageState>;
}
