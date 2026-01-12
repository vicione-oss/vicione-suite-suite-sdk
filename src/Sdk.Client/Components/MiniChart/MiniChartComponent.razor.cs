using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.MiniChart;

public partial class MiniChartComponent : ComponentBase
{
    [Parameter] public MiniChartOrientation Orientation { get; set; }
    [Parameter] public bool ShowPercentages { get; set; }
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; }
}
