using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.CircularGauge;

public partial class CircularGaugeComponent
{
    private double _valueInDegree;
    private double _averageInDegree;
    private double _maxInDegree;
    private string _tooltipPosition = "";
    private string _arrowPositionCssClass = "";

    [Parameter, EditorRequired] public string Name { get; set; }
    [Parameter, EditorRequired] public double Current { get; set; }
    [Parameter, EditorRequired] public double Average { get; set; }
    [Parameter, EditorRequired] public int Maximum { get; set; }
    [Parameter] public int RangeStart { get; set; }
    [Parameter, EditorRequired] public int RangeEnd { get; set; }
    [Parameter, EditorRequired] public string MeasurementUnit { get; set; }
    [Parameter, EditorRequired] public string Color { get; set; }
    [Parameter] public TooltipPosition TooltipPosition { get; set; } = TooltipPosition.Left;

    protected override void OnInitialized()
    {
        _valueInDegree = CalculateDegree(Current, RangeEnd);
        _tooltipPosition = (TooltipPosition == TooltipPosition.Right) ? "right: 0;" : "left: 0;";
        _arrowPositionCssClass = (TooltipPosition == TooltipPosition.Right) ? "top-right" : "top-left";
        _averageInDegree = CalculateRotationAngle(Average, RangeEnd);
        _maxInDegree = CalculateRotationAngle(Maximum, RangeEnd);
    }

    private static double CalculateDegree(double value, int maximumValue)
        => value * 250.0 / maximumValue;

    private static double CalculateRotationAngle(double value, int maximumValue)
        => (value * 250.0 / maximumValue) + 55;
}
