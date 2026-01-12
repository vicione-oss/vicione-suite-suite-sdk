namespace Sdk.Connections.Contracts;

public sealed class Tag(string text, Guid id) : IEquatable<Tag>
{
    public Guid Id { get; init; } = id;
    public string Text { get; set; } = text;
    public bool Protected { get; init; }

    public Tag() : this(string.Empty) { }

    public Tag(string text) : this(text, Guid.NewGuid()) { }

    public bool Equals(Tag? other)
    {
        if (other is null)
            return false;
        return ReferenceEquals(this, other) || Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        return obj is Tag tag && Equals(tag);
    }

    public override int GetHashCode()
        => Id.GetHashCode();

    public override string ToString()
        => Text + (Protected ? " [Protected]" : string.Empty);

    public static bool operator ==(Tag? left, Tag? right)
        => Equals(left, right);

    public static bool operator !=(Tag? left, Tag? right)
        => !(left == right);
}
