namespace Sdk.Client.Wizards.Services;

/// <summary>
/// Item to register a wizard page in a registry
/// </summary>
public interface IWizardPageRegistryItem
{
    /// <summary>
    /// Component type that implements the registed wizard page
    /// </summary>
    Type ComponentType { get; }

    /// <summary>
    /// Descriptor for the registed wizard page
    /// </summary>
    IWizardPageDescriptor Descriptor { get; }

    /// <summary>
    /// State of the registed wizard page
    /// </summary>
    IWizardPageState State { get; }
}
