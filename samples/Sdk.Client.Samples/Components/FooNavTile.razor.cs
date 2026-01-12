using Bogus;
using Microsoft.AspNetCore.Components;
using Sdk.Client.NavTiles.Components;

namespace Sdk.Client.Samples.Components;

public sealed partial class FooNavTile : NavTileBase
{
    private static readonly Faker s_faker = new();

    private readonly string _headline = s_faker.Lorem.Text();
    private readonly string _subline = s_faker.Lorem.Sentence();

    [Parameter]
    public bool WithSubline { get; set; }
}
