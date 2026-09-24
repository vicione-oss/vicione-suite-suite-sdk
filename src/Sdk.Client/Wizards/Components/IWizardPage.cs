using Sdk.Client.Wizards.Services;

namespace Sdk.Client.Wizards.Components;

/// <summary>
/// Marks a component as a wizard page whose state is <typeparamref name="TState"/>.
/// </summary>
public interface IWizardPage<TState> : IComponent where TState : IWizardPageState;
