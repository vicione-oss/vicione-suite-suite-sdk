using Microsoft.AspNetCore.Components;
using Sdk.Client.Colors;

namespace Sdk.Client.Components.MiniChart;

public partial class MiniChartValueComponent : ComponentBase
{
    [CascadingParameter] private MiniChartComponent? MiniChart { get; set; }

    [Parameter, EditorRequired] public double Minimum { get; set; }
    [Parameter, EditorRequired] public double Maximum { get; set; }
    [Parameter, EditorRequired] public double Current { get; set; }
    [Parameter] public IHtmlColor FillColor { get; set; } = StandardHtmlColor.From(StandardColor.LightGreen);
    [Parameter, EditorRequired] public string Label { get; set; }

    protected override void OnInitialized()
    {
        if (MiniChart is null)
        {
            throw new ArgumentNullException(nameof(MiniChartValueComponent),
                $"{nameof(MiniChartValueComponent)} must exist within a {nameof(MiniChartComponent)}");
        }

        base.OnInitialized();
    }

    private bool IsPercentageVisible() => MiniChart is not null && MiniChart.ShowPercentages;
}
