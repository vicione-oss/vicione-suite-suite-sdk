namespace Sdk.Backend.Diagnostics;

/// <summary>
/// Declares that every concrete type implementing the decorated interface (or deriving from the
/// decorated base class) must itself be decorated with an attribute of the given type. Enforced at
/// compile time by the <c>MustDeclareAnalyzer</c>.
/// </summary>
/// <param name="requiredAttributeType">
/// The attribute type that implementing types must declare. A subtype of this attribute satisfies
/// the requirement as well.
/// </param>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = true)]
public sealed class MustDeclareAttribute(Type requiredAttributeType) : Attribute
{
    /// <summary>
    /// Gets the attribute type that implementing types must declare.
    /// </summary>
    public Type RequiredAttributeType { get; } = requiredAttributeType;
}
