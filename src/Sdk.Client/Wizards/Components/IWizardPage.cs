using Microsoft.AspNetCore.Components;
using Sdk.Client.Wizards.Services;

namespace Sdk.Client.Wizards.Components;

/// <summary>
/// Component that implements a wizard page based on <typeparamref name="TState"/>
/// </summary>
public interface IWizardPage<TState> : IComponent where TState : IWizardPageState;
