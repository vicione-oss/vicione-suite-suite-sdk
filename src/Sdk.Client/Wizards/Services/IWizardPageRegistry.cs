using Sdk.Client.Services;
using Sdk.Client.Wizards.Components;

namespace Sdk.Client.Wizards.Services;

/// <summary>
/// Registry for wizard pages
/// </summary>
public interface IWizardPageRegistry : IRegistry<IWizardPageRegistryItem>
{
    /// <summary>
    /// Registers a wizard page in the registry.
    /// </summary>
    /// <param name="descriptor">Descriptor for the wizard page</param>
    /// <param name="state">State for the wizard page</param>
    IWizardPageRegistryItem Add<TComponent, TState>(IWizardPageDescriptor descriptor, TState state)
        where TComponent : class, IWizardPage<TState>
        where TState : IWizardPageState;
}

/// <inheritdoc/>
public interface IWizardPageRegistry<TContext> : IWizardPageRegistry;
