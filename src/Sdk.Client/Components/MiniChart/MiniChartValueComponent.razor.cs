using Sdk.Client.Colors;

namespace Sdk.Client.Components.MiniChart;

/// <summary>
/// Represents a single value or bar within a <see cref="MiniChartComponent"/>.
/// </summary>
public partial class MiniChartValueComponent : ComponentBase
{
    [CascadingParameter] private MiniChartComponent? MiniChart { get; set; }

    /// <summary>
    /// Gets or sets the minimum value of the range for this data point.
    /// </summary>
    [Parameter, EditorRequired] public double Minimum { get; set; }

    /// <summary>
    /// Gets or sets the maximum value of the range for this data point.
    /// </summary>
    [Parameter, EditorRequired] public double Maximum { get; set; }

    /// <summary>
    /// Gets or sets the current value to be visualized.
    /// </summary>
    [Parameter, EditorRequired] public double Current { get; set; }

    /// <summary>
    /// Gets or sets the fill color for the value representation.
    /// </summary>
    [Parameter] public IHtmlColor FillColor { get; set; } = StandardHtmlColor.From(StandardColor.LightGreen);

    /// <summary>
    /// Gets or sets the label displayed for this value.
    /// </summary>
    [Parameter, EditorRequired] public string Label { get; set; }

    /// <summary>
    /// Method invoked when the component is initialized. It ensures that the component
    /// exists within a parent <see cref="MiniChartComponent"/>.
    /// </summary>
    protected override void OnInitialized()
    {
        if (MiniChart is null)
        {
            throw new ArgumentNullException(nameof(MiniChartValueComponent),
                $"{nameof(MiniChartValueComponent)} must exist within a {nameof(MiniChartComponent)}");
        }

        base.OnInitialized();
    }

    private bool IsPercentageVisible() => MiniChart?.ShowPercentages == true;
}
