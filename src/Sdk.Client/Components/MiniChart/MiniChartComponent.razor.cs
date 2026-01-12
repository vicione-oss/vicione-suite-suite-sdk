using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.MiniChart;

/// <summary>
/// A container component for displaying a series of <see cref="MiniChartValueComponent"/> items.
/// </summary>
public partial class MiniChartComponent : ComponentBase
{
    /// <summary>
    /// Gets or sets the orientation of the mini chart, which determines how the child values are arranged.
    /// </summary>
    [Parameter] public MiniChartOrientation Orientation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display percentage values for each data point.
    /// </summary>
    [Parameter] public bool ShowPercentages { get; set; }

    /// <summary>
    /// Gets or sets the child content of the component, which should consist of one or more <see cref="MiniChartValueComponent"/> instances.
    /// </summary>
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; }
}
