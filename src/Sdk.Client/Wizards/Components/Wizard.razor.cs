using System.Globalization;
using Microsoft.AspNetCore.Components;
using Sdk.Client.Wizards.Services;

namespace Sdk.Client.Wizards.Components;

/// <summary>
/// A component that implements a wizard for a specific context.
/// </summary>
/// <typeparam name="TContext">The context type associated with this wizard instance.</typeparam>
public sealed partial class Wizard<TContext> : ComponentBase
{
    private Type? _contentComponentType;
    private readonly Dictionary<string, object> _contentComponentParameters = [];

    /// <summary>
    /// Gets or sets the title displayed at the top of the wizard.
    /// </summary>
    [Parameter, EditorRequired] public string Title { get; set; }

    /// <summary>
    /// Gets or sets the context for the wizard, which is passed to its content pages.
    /// </summary>
    [Parameter, EditorRequired] public TContext Context { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the wizard is currently visible.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets an event callback that is invoked when <see cref="Visible"/> has changed.
    /// </summary>
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is allowed to exit the wizard at any step.
    /// </summary>
    [Parameter] public bool AllowExit { get; set; }

    [Inject] private IWizardContentComponentTypeProvider ContentComponentTypeProvider { get; set; } = default!;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        _contentComponentType = ContentComponentTypeProvider.GetWizardContentComponentType<TContext>();

        if (!(_contentComponentType.IsClass && !_contentComponentType.IsAbstract))
        {
            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                "Component type returned by {0} is not a concrete class",
                nameof(IWizardContentComponentTypeProvider)));
        }

        if (!typeof(IWizardContent<TContext>).IsAssignableFrom(_contentComponentType))
        {
            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                "Component type returned by {0} does not implement {1}",
                nameof(IWizardContentComponentTypeProvider), nameof(IWizardContent<TContext>)));
        }

        _contentComponentParameters.Add(nameof(IWizardContent<TContext>.VisibleChanged),
            EventCallback.Factory.Create<bool>(this, OnVisibleChanged));
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        _contentComponentParameters[nameof(IWizardContent<TContext>.Title)] = Title;
        _contentComponentParameters[nameof(IWizardContent<TContext>.Context)] = Context!;
        _contentComponentParameters[nameof(IWizardContent<TContext>.Visible)] = Visible;
        _contentComponentParameters[nameof(IWizardContent<TContext>.AllowExit)] = AllowExit;
    }

    private async Task OnVisibleChanged(bool value)
    {
        Visible = value;

        if (VisibleChanged.HasDelegate)
            await VisibleChanged.InvokeAsync(value);
    }
}
