using Microsoft.AspNetCore.Components;
using Sdk.Client.Colors;

namespace Sdk.Client.ControlPanels.Components;

public partial class IconAndValueComponent<T> : ComponentBase
{
    [Parameter, EditorRequired] public string IconSrc { get; set; }
    [Parameter, EditorRequired] public string IconAlt { get; set; }
    [Parameter, EditorRequired] public T Value { get; set; }
    [Parameter] public IHtmlColor? ValueColor { get; set; }
    [Parameter] public Func<T, string>? ValueFormatter { get; set; }
    [Parameter] public string? MeasurementUnit { get; set; }
}
