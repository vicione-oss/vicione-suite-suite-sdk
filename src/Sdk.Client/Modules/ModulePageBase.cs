using Microsoft.AspNetCore.Components;
using Sdk.Client.NotificationArea.Services;
using Sdk.Client.Services;

namespace Sdk.Client.Modules;

public abstract class ModulePageBase<TClientModule> : ModuleComponentBase<TClientModule>
    where TClientModule : IClientModule
{
    [Inject] protected ILayoutService LayoutService { get; set; } = default!;
    [Inject] protected IActiveNotificationElementPolicy ActiveNotificationElementPolicy { get; set; } = default!;

    protected virtual string PageTitle { get; } = typeof(TClientModule).Name;

    protected sealed override void OnInitialized()
    {
        base.OnInitialized();

        OnModulePageInitialized();

        OnAfterInitialized();
    }

    private void OnModulePageInitialized()
    {
        if (Equals(LayoutService.TitleBarAppName, PageTitle))
            return; // no change

        LayoutService.TitleBarAppName = PageTitle;
        if (!string.IsNullOrEmpty(LayoutService.TitleBarAppName))
            LayoutService.TitleBarText = null;

        ActiveNotificationElementPolicy.NoneActive();
    }

    /// <summary>
    /// Method invoked after OnInitialized was executed
    /// </summary>
    /// <returns></returns>
    protected virtual void OnAfterInitialized()
    {
    }
}
