using Microsoft.AspNetCore.Components;
using Sdk.Client.NavTiles.Components;

namespace Sdk.Client.Samples.Shared.Pages.NavTiles;

public sealed partial class NavTilesPage : ComponentBase
{
    private readonly NavTileState _fooNavTileState = new();

    private bool _fooNavTileWithSubline = true;
}
