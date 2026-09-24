using Sdk.Client.NotificationArea.Services;
using Sdk.Client.Services;

namespace Sdk.Client.Modules;

/// <summary>
/// An abstract base class for top-level module pages.
/// </summary>
public abstract class ModulePageBase<TClientModule> : ModuleComponentBase<TClientModule>
    where TClientModule : IClientModule
{
    /// <summary>
    /// Gets or sets the injected service for controlling the main application layout.
    /// </summary>
    [Inject] protected ILayoutService LayoutService { get; set; } = default!;

    /// <summary>
    /// Gets or sets the injected policy service for managing the active state of notification elements.
    /// </summary>
    [Inject] protected IActiveNotificationElementPolicy ActiveNotificationElementPolicy { get; set; } = default!;

    /// <summary>
    /// Gets the title for the page, which is displayed in the application's title bar.
    /// The default value is the name of the client module type.
    /// </summary>
    protected virtual string PageTitle { get; } = typeof(TClientModule).Name;

    /// <summary>
    /// Shows <see cref="PageTitle"/> in the title bar and, if the title changed, deactivates all notification elements;
    /// then calls <see cref="OnAfterInitialized"/>.
    /// </summary>
    protected sealed override void OnInitialized()
    {
        base.OnInitialized();

        OnModulePageInitialized();

        OnAfterInitialized();
    }

    private void OnModulePageInitialized()
    {
        if (Equals(LayoutService.TitleBarAppName, PageTitle))
            return;

        LayoutService.TitleBarAppName = PageTitle;
        if (!string.IsNullOrEmpty(LayoutService.TitleBarAppName))
            LayoutService.TitleBarText = null;

        ActiveNotificationElementPolicy.NoneActive();
    }

    /// <summary>
    /// Invoked after the base <see cref="OnInitialized"/> logic has been executed.
    /// Override this method in derived classes to perform component-specific initialization.
    /// </summary>
    protected virtual void OnAfterInitialized()
    {
    }
}
