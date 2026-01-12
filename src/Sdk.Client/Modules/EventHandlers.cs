using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Modules;

[EventHandler("onmouseenter", typeof(EventArgs), true, true)]
[EventHandler("onmouseleave", typeof(EventArgs), true, true)]
public static class EventHandlers;
