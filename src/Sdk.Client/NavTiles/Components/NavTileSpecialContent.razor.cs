namespace Sdk.Client.NavTiles.Components;

/// <summary>
/// Represents a special content layout for a navigation tile, featuring distinct left and right content areas.
/// </summary>
[ExcludeFromCodeCoverage]
public partial class NavTileSpecialContent : ComponentBase
{
    /// <summary>
    /// Gets or sets the main headline text for the tile.
    /// </summary>
    [Parameter, EditorRequired] public string Headline { get; set; }

    /// <summary>
    /// Gets or sets the optional subline text for the tile.
    /// </summary>
    [Parameter] public string? Subline { get; set; }

    /// <summary>
    /// Gets or sets the content to be displayed in the left-side area of the tile.
    /// </summary>
    [Parameter] public RenderFragment? LeftContent { get; set; }

    /// <summary>
    /// Gets or sets an optional description for the left-side content, often used for accessibility or tooltips.
    /// </summary>
    [Parameter] public string? LeftContentDescription { get; set; }

    /// <summary>
    /// Gets or sets the content to be displayed in the right-side area of the tile.
    /// </summary>
    [Parameter] public RenderFragment? RightContent { get; set; }

    /// <summary>
    /// Gets or sets an optional description for the right-side content, often used for accessibility or tooltips.
    /// </summary>
    [Parameter] public string? RightContentDescription { get; set; }
}
