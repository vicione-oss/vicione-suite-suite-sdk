using Sdk.Client.Enums;

namespace Sdk.Client.Components.MaterialDesignIcon;

/// <summary>
/// A component that renders an icon from the Material Design Icons webfont.
/// </summary>
public sealed partial class MaterialDesignIconComponent : ComponentBase
{
    /// <summary>
    /// Gets or sets the specific icon to be displayed, selected from the <see cref="MaterialDesignIconName"/> enum.
    /// </summary>
    [Parameter, EditorRequired]
    public MaterialDesignIconName Name { get; set; }
}
