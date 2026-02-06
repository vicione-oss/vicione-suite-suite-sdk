using Bogus;
using Microsoft.AspNetCore.Components;
using Sdk.Client.NavTiles.Components;

namespace Sdk.Client.Samples.Shared.Pages.NavTiles;

public sealed partial class FooNavTile : NavTileBase
{
    private static readonly Faker s_faker = new();

    private readonly string _headline = s_faker.Lorem.Text();
    private readonly string _subline = s_faker.Lorem.Sentence();

    private readonly Uri _iconUrl = new($"_content/{typeof(FooNavTile).Assembly.GetName().Name}/foo-nav-tile/icon.svg", UriKind.Relative);

    [Parameter]
    public bool WithSubline { get; set; }
}
