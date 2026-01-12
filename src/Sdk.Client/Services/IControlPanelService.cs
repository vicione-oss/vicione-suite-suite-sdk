using Sdk.Client.ControlPanels.Models;
using Sdk.Client.Modules;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Defines a service for managing the edit lifecycle of a control panel.
/// </summary>
[Obsolete("Not used anymore, use virtual methods in ControlPanelBase instead")]
public interface IControlPanelService
{
    /// <summary>
    /// Gets a value indicating whether the control panel has unsaved changes.
    /// </summary>
    bool IsDirty { get; }

    /// <summary>
    /// Occurs when a request to save the control panel's changes is initiated.
    /// </summary>
    event Func<Task<ISaveResult>>? OnSave;

    /// <summary>
    /// Occurs when a request to cancel the editing process and discard changes is initiated.
    /// </summary>
    event Func<Task>? OnCancel;

    /// <summary>
    /// Occurs when the control panel enters edit mode.
    /// </summary>
    event Func<Task>? OnBeginEdit;

    /// <summary>
    /// Occurs when the edit mode is cancelled.
    /// </summary>
    event Func<Task>? OnCancelEdit;

    /// <summary>
    /// Puts the control panel into an editable state.
    /// </summary>
    Task BeginEdit();

    /// <summary>
    /// Cancels the edit mode and discards any unsaved changes.
    /// </summary>
    Task CancelEdit();

    /// <summary>
    /// Completes the edit mode, typically by saving the changes.
    /// </summary>
    Task<ControlPanelStateFinishResult> FinishEdit();
}
