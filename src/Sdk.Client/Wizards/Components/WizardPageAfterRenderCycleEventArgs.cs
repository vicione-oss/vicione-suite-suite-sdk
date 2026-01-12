namespace Sdk.Client.Wizards.Components;

/// <summary>
/// Arguments for <see cref="WizardPage{TState}.OnAfterRenderCycle"/> event
/// </summary>
public sealed class WizardPageAfterRenderCycleEventArgs(bool firstRender) : EventArgs
{
    /// <summary>
    /// <see langword="true"/> when the render cycle is the first render, otherwise <see langword="false"/>.
    /// </summary>
    public bool FirstRender => firstRender;
}
