namespace Sdk.Messaging;

/// <summary>
/// Defines the type of a Create, Read, Update, Delete (CRUD) operation.
/// </summary>
public enum CrudAction
{
    /// <summary>
    /// Indicates that a new entity was created.
    /// </summary>
    Created,

    /// <summary>
    /// Indicates that an existing entity was updated.
    /// </summary>
    Updated,

    /// <summary>
    /// Indicates that an entity was deleted.
    /// </summary>
    Deleted
}
