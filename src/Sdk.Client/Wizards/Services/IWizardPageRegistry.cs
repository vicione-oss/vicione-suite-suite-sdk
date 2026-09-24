using Sdk.Client.Services;
using Sdk.Client.Wizards.Components;

namespace Sdk.Client.Wizards.Services;

/// <summary>
/// Holds the pages of a wizard.
/// </summary>
public interface IWizardPageRegistry : IRegistry<IWizardPageRegistryItem>
{
    /// <summary>
    /// Registers a wizard page in the registry.
    /// </summary>
    /// <param name="descriptor">Describes how the page is presented.</param>
    /// <param name="state">The page's state.</param>
    IWizardPageRegistryItem Add<TComponent, TState>(IWizardPageDescriptor descriptor, TState state)
        where TComponent : class, IWizardPage<TState>
        where TState : IWizardPageState;
}

/// <inheritdoc/>
public interface IWizardPageRegistry<TContext> : IWizardPageRegistry;
