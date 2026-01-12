using Sdk.Client.NavTiles.Attributes;
using Sdk.Client.NavTiles.Components;

namespace TestModule.Client.NavTiles;

[InitialNavTile<TestOtherEditorClientModule>(LinkTarget = "/other-editor")]
public partial class OtherEditorNavTile : NavTileBase;