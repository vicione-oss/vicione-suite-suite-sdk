using Microsoft.AspNetCore.Components;

namespace Sdk.Client.NavTiles.Components;

public class NavTileBase : ComponentBase, INavTile
{
    [Parameter, EditorRequired] public NavTileState State { get; set; }

    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;

    public void ContentLoading() => State.BeginLoading();
    public void ContentReady() => State.EndLoading();

    public virtual void Click()
    {
        if (string.IsNullOrEmpty(State.LinkTarget))
            return;

        NavigationManager.NavigateTo(State.LinkTarget);
    }
}
