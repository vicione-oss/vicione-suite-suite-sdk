using Sdk.Client.ControlPanels.Models;
using Sdk.Client.Modules;

namespace Sdk.Client.ControlPanels.Services;

public interface IControlPanelService
{
    bool IsDirty { get; }
    event Func<Task<ISaveResult>>? OnSave;
    event Func<Task>? OnCancel;
    event Func<Task>? OnBeginEdit;
    event Func<Task>? OnCancelEdit;
    Task BeginEdit();
    Task CancelEdit();
    Task<ControlPanelStateFinishResult> FinishEdit();
}
