using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.Modules;
using Sdk.Client.NotificationArea.Components;
using Sdk.Client.NotificationArea.Extensions;
using Sdk.Client.NotificationArea.Services;

namespace Sdk.Client.NotificationArea.Attributes;

/// <summary>
/// Marks a class implementing <see cref="NotificationElementBase{TState}"/> as an implementation that is automatically discovered
/// when <see cref="IServiceCollectionExtensions.AddNotificationElements{TClientModule}(IServiceCollection)"/> is called.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class InitialNotificationElementAttribute<TClientModule> : Attribute
    where TClientModule : IClientModule
{
    private Guid _id = Guid.NewGuid();

    /// <summary>
    /// GUID string for uniquely identifying the notification element in <see cref="INotificationElementRegistry{TClientModule}"/> operations.
    ///
    /// The default value is a random GUID string.
    /// </summary>
    public string Id
    {
        get => _id.ToString();
        set => _id = Guid.Parse(value);
    }

    /// <summary>
    /// Specifies the default value assigned to <see cref="INotificationElementState.Visible"/>.
    /// </summary>
    public bool Visible { get; set; } = Constants.NotificationElementVisibleDefault;

    /// <summary>
    /// Specifies the default value assigned to <see cref="INotificationElementRegistryItem.Position"/>.
    /// </summary>
    public int Position { get; set; } = Constants.NotificationElementPositionDefault;
}
