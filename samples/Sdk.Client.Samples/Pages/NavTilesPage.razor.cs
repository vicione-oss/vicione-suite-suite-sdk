using Microsoft.AspNetCore.Components;
using Sdk.Client.NavTiles.Components;

namespace Sdk.Client.Samples.Pages;

public sealed partial class NavTilesPage : ComponentBase
{
    private readonly NavTileState _fooNavTileState = new();

    private bool _fooNavTileWithSubline = true;
}
