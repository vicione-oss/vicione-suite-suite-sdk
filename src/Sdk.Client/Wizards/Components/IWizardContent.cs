namespace Sdk.Client.Wizards.Components;

/// <summary>
/// Component that implements content suitable for a wizard associated with <typeparamref name="TContext"/>.
/// </summary>
/// <typeparam name="TContext">Context associated with the wizard</typeparam>
public interface IWizardContent<TContext> : IComponent
{
    /// <summary>
    /// Gets or sets the title of the wizard.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Gets or sets the context associated with the wizard.
    /// </summary>
    TContext Context { get; set; }

    /// <summary>
    /// Gets or sets the visibility of the wizard.
    /// </summary>
    bool Visible { get; set; }

    /// <summary>
    /// Gets or sets the callback that is invoked when <see cref="Visible"/> changes.
    /// </summary>
    EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the wizard allows exit at any time.
    /// </summary>
    bool AllowExit { get; set; }
}
