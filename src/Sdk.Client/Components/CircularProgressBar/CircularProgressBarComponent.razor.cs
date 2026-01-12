using Microsoft.AspNetCore.Components;
using Sdk.Client.Colors;

namespace Sdk.Client.Components.CircularProgressBar;

/// <summary>
/// A component that displays a value as a circular progress bar.
/// </summary>
public partial class CircularProgressBarComponent : ComponentBase
{
    /// <summary>
    /// Gets or sets the minimum value of the range, representing the start of the progress bar.
    /// </summary>
    [Parameter, EditorRequired] public double Minimum { get; set; }

    /// <summary>
    /// Gets or sets the maximum value of the range, representing a full progress bar.
    /// </summary>
    [Parameter, EditorRequired] public double Maximum { get; set; }

    /// <summary>
    /// Gets or sets the current value to be visualized.
    /// </summary>
    [Parameter, EditorRequired] public double Current { get; set; }

    /// <summary>
    /// Gets or sets the color of the progress arc. If not set, a default color is used.
    /// </summary>
    [Parameter] public IHtmlColor? ProgressSpinnerColor { get; set; }

    /// <summary>
    /// Gets or sets the label displayed in the center of the progress bar.
    /// </summary>
    [Parameter, EditorRequired] public string Label { get; set; }
}
