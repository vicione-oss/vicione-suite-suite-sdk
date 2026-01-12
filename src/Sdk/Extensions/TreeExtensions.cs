namespace Sdk.Extensions;

/// <summary>
/// Provides extension methods for working with tree-like data structures.
/// </summary>
public static class TreeExtensions
{
    /// <summary>
    /// Converts a flat collection of items into a hierarchical tree structure.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the collection does not contain exactly one root element.</exception>
    public static TreeNode<TItem>? ToTree<TItem, TKey>(
        this IEnumerable<TItem> collection,
        Func<TItem, TKey> idSelector,
        Func<TItem, TKey?> parentIdSelector,
        TKey? rootId = default)
    {
        var collectionArray = collection.ToArray();
        try
        {
            var tree = collectionArray.Select(r => new TreeNode<TItem>(r)).ToList();
            if (tree.Count == 0)
                return null;
            if (tree.Count == 1)
                return tree[0];

            var lookup = tree.ToLookup(node => parentIdSelector(node.Item));
            foreach (var node in tree)
            {
                var kids = lookup[idSelector(node.Item)];
                if (kids.TryGetNonEnumeratedCount(out var count))
                {
                    if (count > 0)
                        node.Children = [.. kids];
                }
                else
                {
                    var enumeratedKids = kids.ToList();
                    if (enumeratedKids.Count > 0)
                    {
                        node.Children = enumeratedKids;
                    }
                }
            }

            return tree.Single(node => EqualityComparer<TKey?>.Default.Equals(parentIdSelector(node.Item), rootId));
        }
        catch (InvalidOperationException e)
        {
            var rootItemCount = collectionArray.Count(i => EqualityComparer<TKey?>.Default.Equals(parentIdSelector(i), rootId));

            throw new InvalidOperationException($"The collection must have one root element, but {rootItemCount} returned.", e);
        }
    }

    /// <summary>
    /// Traverses a tree structure in breadth-first order, yielding each item along with its parent.
    /// </summary>
    public static IEnumerable<(T Item, T? Parent)> TraverseBreadthFirstWithParent<T>(this T parent, Func<T, IEnumerable<T>> childrenSelector)
    {
        Queue<(T Item, T? Parent)> items = new();
        items.Enqueue((parent, default));
        while (items.TryDequeue(out var tuple))
        {
            yield return tuple;

            foreach (var child in childrenSelector(tuple.Item))
                items.Enqueue((child, tuple.Item));
        }
    }

    /// <summary>
    /// Traverses a tree structure in depth-first order, yielding each item along with its path from the root.
    /// </summary>
    public static IEnumerable<(T Item, IReadOnlyCollection<T> Path)> TraverseDepthFirstWithPath<T>(this T parent, Func<T, IEnumerable<T>> childrenSelector)
    {
        Stack<T> path = new();
        Stack<(T Item, int PopCount)> items = new();
        items.Push((parent, 0));
        while (items.TryPop(out var tuple))
        {
            yield return (tuple.Item, path.Reverse().ToList());

            var appendPath = true;
            foreach (var child in childrenSelector(tuple.Item).Reverse())
            {
                var popCount = 0;
                if (appendPath)
                {
                    appendPath = false;
                    path.Push(tuple.Item);
                    popCount = tuple.PopCount + 1;
                    tuple.PopCount = 0;
                }
                items.Push((child, popCount));
            }

            for (var i = 0; i < tuple.PopCount; i++)
                path.TryPop(out _);
        }
    }

    /// <summary>
    /// Flattens a hierarchical structure into a single collection, starting from a single root item.
    /// </summary>
    public static IEnumerable<T> Flatten<T>(this T parent, Func<T, IEnumerable<T>> childrenSelector)
        => Enumerable.Repeat(parent, 1).Flatten(childrenSelector);

    /// <summary>
    /// Flattens a collection of hierarchical structures into a single collection.
    /// </summary>
    public static IEnumerable<T> Flatten<T>(this IEnumerable<T> enumerable, Func<T, IEnumerable<T>> childrenSelector)
    {
        Queue<T> items = new(enumerable);
        while (items.TryDequeue(out var item))
        {
            yield return item;

            foreach (var child in childrenSelector(item))
                items.Enqueue(child);
        }
    }
}

/// <summary>
/// Represents a node in a tree structure, containing an item and its children.
/// </summary>
public class TreeNode<T>(T item)
{
    /// <summary>
    /// Gets the item contained in this tree node.
    /// </summary>
    public T Item { get; } = item;

    /// <summary>
    /// Gets or sets the collection of child nodes for this tree node.
    /// </summary>
    public ICollection<TreeNode<T>>? Children { get; internal set; }
}
