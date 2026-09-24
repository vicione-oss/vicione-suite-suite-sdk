namespace Sdk.Backend.Artifacts;

/// <summary>
/// Represents a range of artifact query results, including start and end positions and the total number of items.
/// </summary>
public interface IArtifactQueryRange
{
    /// <summary>
    /// Gets the zero-based index of the first item in the result range.
    /// </summary>
    int StartPosition { get; }

    /// <summary>
    /// Gets the zero-based index of the last item in the result range.
    /// </summary>
    int EndPosition { get; }

    /// <summary>
    /// Gets the total number of items available, regardless of range.
    /// </summary>
    int Total { get; }

    /// <summary>
    /// Gets the source the range belongs to.
    /// </summary>
    string Source { get; }
}
