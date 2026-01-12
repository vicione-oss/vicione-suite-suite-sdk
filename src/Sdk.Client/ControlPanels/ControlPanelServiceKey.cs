using Microsoft.AspNetCore.Components;
using Sdk.Client.ControlPanels.Components;

namespace Sdk.Client.ControlPanels;

/// <summary>
/// Class type that is used as service key in registrations of services related to <typeparamref name="TControlPanel"/>.
/// </summary>
public sealed class ControlPanelServiceKey<TControlPanel>
    where TControlPanel : ComponentBase, IControlPanel;
