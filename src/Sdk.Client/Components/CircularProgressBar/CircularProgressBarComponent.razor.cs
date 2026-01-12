using Microsoft.AspNetCore.Components;
using Sdk.Client.Colors;

namespace Sdk.Client.Components.CircularProgressBar;

public partial class CircularProgressBarComponent : ComponentBase
{
    [Parameter, EditorRequired] public double Minimum { get; set; }
    [Parameter, EditorRequired] public double Maximum { get; set; }
    [Parameter, EditorRequired] public double Current { get; set; }
    [Parameter] public IHtmlColor? ProgressSpinnerColor { get; set; }
    [Parameter, EditorRequired] public string Label { get; set; }
}
