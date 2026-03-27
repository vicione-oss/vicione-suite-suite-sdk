namespace Sdk.Backend.Modules;

/// <summary>
/// Defines a copy behavior override for resources matching a specific glob pattern.
/// </summary>
/// <param name="Pattern">
/// A glob pattern matched against the relative path of resource files within the resource directory.
/// Supports standard glob syntax: <c>*</c> (any filename), <c>**</c> (recursive directories),
/// <c>?</c> (single character), e.g. <c>"*.xml"</c>, <c>"config/**"</c>, <c>"certs/*.pem"</c>.
/// </param>
/// <param name="Behavior">The <see cref="ResourceCopyBehavior"/> to apply to matching files.</param>
public readonly record struct ResourceRule(string Pattern, ResourceCopyBehavior Behavior);
