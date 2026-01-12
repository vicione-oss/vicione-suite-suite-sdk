using Sdk.Client.Wizards.Components;
using Sdk.Client.Wizards.Services;

namespace Sdk.Client.Wizards.Builders;

/// <summary>
/// Builder for a page implemented by <typeparamref name="TComponent"/> based on <typeparamref name="TState"/>
/// in a wizard associated with <typeparamref name="TContext"/>.
/// </summary>
public interface IWizardPageBuilder<TContext, TComponent, TState>
    where TComponent : class, IWizardPage<TState>
    where TState : IWizardPageState
{
    /// <summary>
    /// Configures auto-discovery which adds <typeparamref name="TComponent"/> to
    /// <see cref="IWizardPageRegistry{TContext}"/> with the given <typeparamref name="TDescriptor"/>
    /// and <typeparamref name="TState"/>.
    /// </summary>
    /// <remarks>
    /// The method will register <see cref="IWizardPageDescriptor"/> and
    /// <typeparamref name="TState"/> as <see cref="WizardPageServiceKey{TContext, TComponent}">keyed service</see>.
    /// </remarks>
    IWizardPageBuilder<TContext, TComponent, TState> WithAutoDiscovery<TDescriptor>()
        where TDescriptor : class, IWizardPageDescriptor;

    /// <summary>
    /// Configures <typeparamref name="TSaveHandler"/> for handling save requests in context of <typeparamref name="TComponent"/>
    /// </summary>
    IWizardPageBuilder<TContext, TComponent, TState> WithSaveHandler<TSaveHandler>()
        where TSaveHandler : class, IWizardPageSaveHandler<TState>;

    /// <summary>
    /// Configures <typeparamref name="TCancelHandler"/> for handling cancel requests in context of <typeparamref name="TComponent"/>
    /// </summary>
    IWizardPageBuilder<TContext, TComponent, TState> WithCancelHandler<TCancelHandler>()
        where TCancelHandler : class, IWizardPageCancelHandler<TState>;

    /// <summary>
    /// Configures <typeparamref name="TResetHandler"/> for handling reset requests in context of <typeparamref name="TComponent"/>
    /// </summary>
    IWizardPageBuilder<TContext, TComponent, TState> WithResetHandler<TResetHandler>()
        where TResetHandler : class, IWizardPageResetHandler<TState>;
}
