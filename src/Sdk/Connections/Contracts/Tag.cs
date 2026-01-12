namespace Sdk.Connections.Contracts;

/// <summary>
/// Represents a tag that can be used to categorize or label entities, such as connections.
/// </summary>
public sealed class Tag(string text, Guid id) : IEquatable<Tag>
{
    /// <summary>
    /// Gets the unique identifier of the tag.
    /// </summary>
    public Guid Id { get; init; } = id;

    /// <summary>
    /// Gets or sets the display text of the tag.
    /// </summary>
    public string Text { get; set; } = text;

    /// <summary>
    /// Gets a value indicating whether the tag is protected from user modification.
    /// </summary>
    public bool Protected { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tag"/> class with an empty text and a new GUID.
    /// </summary>
    public Tag() : this(string.Empty) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tag"/> class with the specified text and a new GUID.
    /// </summary>
    public Tag(string text) : this(text, Guid.NewGuid()) { }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type based on their IDs.
    /// </summary>
    public bool Equals(Tag? other)
    {
        if (other is null)
            return false;
        return ReferenceEquals(this, other) || Id.Equals(other.Id);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current tag.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        return obj is Tag tag && Equals(tag);
    }

    /// <summary>
    /// Returns the hash code for this instance, which is the hash code of its ID.
    /// </summary>
    public override int GetHashCode()
        => Id.GetHashCode();

    /// <summary>
    /// Returns a string that represents the current tag, including its protection status.
    /// </summary>
    public override string ToString()
        => Text + (Protected ? " [Protected]" : string.Empty);

    /// <summary>
    /// Compares two <see cref="Tag"/> objects for equality.
    /// </summary>
    public static bool operator ==(Tag? left, Tag? right)
        => Equals(left, right);

    /// <summary>
    /// Compares two <see cref="Tag"/> objects for inequality.
    /// </summary>
    public static bool operator !=(Tag? left, Tag? right)
        => !(left == right);
}
