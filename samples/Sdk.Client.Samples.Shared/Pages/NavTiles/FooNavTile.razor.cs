using Bogus;
using Microsoft.AspNetCore.Components;
using Sdk.Client.NavTiles.Components;

namespace Sdk.Client.Samples.Shared.Pages.NavTiles;

// A navigation tile derives from NavTileBase, which supplies the State parameter and the default click behaviour
// (navigate to State.LinkTarget). In a module, a tile carries [InitialNavTile] so the host finds and registers it.
public sealed partial class FooNavTile : NavTileBase
{
    private static readonly Faker s_faker = new();

    // Random text shows how the tile lays out headlines and sublines of varying length.
    private readonly string _headline = s_faker.Lorem.Text();
    private readonly string _subline = s_faker.Lorem.Sentence();

    // Files in a Razor class library's wwwroot are served under _content/{assembly name}/.
    private readonly Uri _iconUrl = new($"_content/{typeof(FooNavTile).Assembly.GetName().Name}/foo-nav-tile/icon.svg", UriKind.Relative);

    [Parameter]
    public bool WithSubline { get; set; }
}
