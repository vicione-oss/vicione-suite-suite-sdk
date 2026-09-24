namespace Sdk.Client.Wizards.Services;

/// <summary>
/// A wizard page registered in an <see cref="IWizardPageRegistry"/>.
/// </summary>
public interface IWizardPageRegistryItem
{
    /// <summary>
    /// Gets the component type that renders the page.
    /// </summary>
    Type ComponentType { get; }

    /// <summary>
    /// Gets the page's descriptor.
    /// </summary>
    IWizardPageDescriptor Descriptor { get; }

    /// <summary>
    /// Gets the page's state.
    /// </summary>
    IWizardPageState State { get; }
}
