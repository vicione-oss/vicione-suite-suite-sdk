using Sdk.Client.ControlPanels.Components;
using Sdk.Client.ControlPanels.Extensions;
using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.ControlPanels.Attributes;

/// <summary>
/// Links a class implementing <see cref="ControlPanelBase{TState}"/> to a category described by
/// <typeparamref name="TControlPanelCategoryDescriptor"/>.
/// </summary>
/// <remarks>
/// This configuration affects control panels registered via
/// <see cref="IServiceCollectionExtensions.AddControlPanel{TClientModule, TControlPanel, TState}"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
[ExcludeFromCodeCoverage]
public sealed class ControlPanelCategoryAttribute<TControlPanelCategoryDescriptor> : Attribute
    where TControlPanelCategoryDescriptor : IControlPanelCategoryDescriptor;
