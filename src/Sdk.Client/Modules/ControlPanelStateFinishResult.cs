using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.Modules;

/// <summary>
/// Represents the result of finishing an edit operation in a control panel.
/// </summary>
[Obsolete("Dependency of " + nameof(IControlPanelService) + ", not used anymore")]
public sealed class ControlPanelStateFinishResult(bool success, string? error = null)
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; } = success;

    /// <summary>
    /// Gets the error message if the operation failed, otherwise <c>null</c>.
    /// </summary>
    public string? Error { get; } = error;
}
