using Microsoft.AspNetCore.Components;
using Sdk.Client.NavTiles.Components;

namespace Sdk.Client.Samples.Shared.Pages.NavTiles;

public sealed partial class NavTilesPage : ComponentBase
{
    // In the suite, the host keeps each tile's state in the module's INavTileRegistry; this page owns one directly so the
    // tile can be shown on its own.
    private readonly NavTileState _fooNavTileState = new();

    private bool _fooNavTileWithSubline = true;
}
