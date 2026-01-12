namespace Sdk.Client.Contracts;

/// <summary>
/// Specifies the scope and lifecycle of a web resource.
/// </summary>
public enum ResourceDeclaration
{
    /// <summary>
    /// The resource is local to a component and will be unloaded when the component is disposed.
    /// </summary>
    Local,

    /// <summary>
    /// The resource is global to the application and will not be unloaded automatically with a single component.
    /// </summary>
    Global
}
