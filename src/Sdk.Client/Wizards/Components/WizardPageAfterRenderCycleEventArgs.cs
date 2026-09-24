namespace Sdk.Client.Wizards.Components;

/// <summary>
/// Arguments of <see cref="WizardPage{TState}.OnAfterRenderCycle"/>.
/// </summary>
public sealed class WizardPageAfterRenderCycleEventArgs(bool firstRender) : EventArgs
{
    /// <summary>
    /// Gets whether this is the page's first render.
    /// </summary>
    public bool FirstRender => firstRender;
}
