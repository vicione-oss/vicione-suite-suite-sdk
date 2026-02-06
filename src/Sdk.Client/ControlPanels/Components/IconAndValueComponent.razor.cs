using Microsoft.AspNetCore.Components;
using Sdk.Client.Colors;

namespace Sdk.Client.ControlPanels.Components;

/// <summary>
/// A component that displays an icon next to a formatted value and an optional measurement unit.
/// </summary>
public partial class IconAndValueComponent<T> : ComponentBase
{
    /// <summary>
    /// Gets or sets the source URL for the icon image.
    /// </summary>
    [Parameter, EditorRequired] public Uri IconUrl { get; set; }

    /// <summary>
    /// Gets or sets the alternative text for the icon image, used for accessibility.
    /// </summary>
    [Parameter, EditorRequired] public string IconAlt { get; set; }

    /// <summary>
    /// Gets or sets the value to be displayed.
    /// </summary>
    [Parameter, EditorRequired] public T Value { get; set; }

    /// <summary>
    /// Gets or sets the optional color for the displayed value.
    /// </summary>
    [Parameter] public IHtmlColor? ValueColor { get; set; }

    /// <summary>
    /// Gets or sets an optional function to format the <see cref="Value"/> for display.
    /// If not provided, the value's default `ToString()` method is used.
    /// </summary>
    [Parameter] public Func<T, string>? ValueFormatter { get; set; }

    /// <summary>
    /// Gets or sets the optional unit of measurement to be displayed next to the value.
    /// </summary>
    [Parameter] public string? MeasurementUnit { get; set; }
}
