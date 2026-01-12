namespace Sdk.Client.Modules;

public sealed class ControlPanelStateFinishResult(bool success, string? error = null)
{
    public bool Success { get; } = success;
    public string? Error { get; } = error;
}
