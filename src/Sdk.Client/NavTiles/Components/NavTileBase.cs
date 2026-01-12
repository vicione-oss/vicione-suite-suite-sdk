using Microsoft.AspNetCore.Components;

namespace Sdk.Client.NavTiles.Components;

/// <summary>
/// Provides a base implementation for a navigation tile component.
/// </summary>
public class NavTileBase : ComponentBase, INavTile
{
    /// <summary>
    /// Gets or sets the state object that holds the dynamic data for the navigation tile.
    /// </summary>
    [Parameter, EditorRequired] public NavTileState State { get; set; }

    /// <summary>
    /// Gets or sets the injected service for handling URI navigation.
    /// </summary>
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;

    /// <summary>
    /// Signals the beginning of a content loading operation, putting the tile into a loading state.
    /// </summary>
    public void ContentLoading() => State.BeginLoading();

    /// <summary>
    /// Signals the end of a content loading operation, taking the tile out of its loading state.
    /// </summary>
    public void ContentReady() => State.EndLoading();

    /// <summary>
    /// Handles the click action for the tile. The default implementation navigates to the URL
    /// specified in <see cref="NavTileState.LinkTarget"/>.
    /// </summary>
    public virtual void Click()
    {
        if (string.IsNullOrEmpty(State.LinkTarget))
            return;

        NavigationManager.NavigateTo(State.LinkTarget);
    }
}
