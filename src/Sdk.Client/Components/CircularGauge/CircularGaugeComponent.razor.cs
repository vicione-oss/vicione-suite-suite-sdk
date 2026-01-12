using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.CircularGauge;

/// <summary>
/// A component that displays a value as a circular gauge.
/// </summary>
public partial class CircularGaugeComponent
{
    private double _valueInDegree;
    private double _averageInDegree;
    private double _maxInDegree;
    private string _tooltipPosition = "";
    private string _arrowPositionCssClass = "";

    /// <summary>
    /// Gets or sets the name of the value being measured, displayed within the gauge.
    /// </summary>
    [Parameter, EditorRequired] public string Name { get; set; }

    /// <summary>
    /// Gets or sets the current value to be displayed on the gauge.
    /// </summary>
    [Parameter, EditorRequired] public double Current { get; set; }

    /// <summary>
    /// Gets or sets the average value, indicated by a marker on the gauge's arc.
    /// </summary>
    [Parameter, EditorRequired] public double Average { get; set; }

    /// <summary>
    /// Gets or sets the maximum value achieved, indicated by a marker on the gauge's arc.
    /// </summary>
    [Parameter, EditorRequired] public int Maximum { get; set; }

    /// <summary>
    /// Gets or sets the starting value of the gauge's range.
    /// </summary>
    [Parameter] public int RangeStart { get; set; }

    /// <summary>
    /// Gets or sets the end value of the gauge's range, representing 100% of the arc.
    /// </summary>
    [Parameter, EditorRequired] public int RangeEnd { get; set; }

    /// <summary>
    /// Gets or sets the unit of measurement for the values, displayed in the tooltip.
    /// </summary>
    [Parameter, EditorRequired] public string MeasurementUnit { get; set; }

    /// <summary>
    /// Gets or sets the color of the gauge's arc as a CSS color string.
    /// </summary>
    [Parameter, EditorRequired] public string Color { get; set; }

    /// <summary>
    /// Gets or sets the position of the tooltip relative to the gauge.
    /// </summary>
    [Parameter] public TooltipPosition TooltipPosition { get; set; } = TooltipPosition.Left;

    /// <inheritdoc/>
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
