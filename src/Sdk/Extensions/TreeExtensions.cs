namespace Sdk.Extensions;

public static class TreeExtensions
{
    /// <summary>
    /// Generates tree of items from item list
    /// Inspired by: https://stackoverflow.com/questions/19648166/nice-universal-way-to-convert-list-of-items-to-tree
    /// </summary>
    /// 
    /// <typeparam name="TItem">Type of item in collection</typeparam>
    /// <typeparam name="TKey">Type of parent_id</typeparam>
    /// 
    /// <param name="collection">Collection of items</param>
    /// <param name="idSelector">Function extracting item's id</param>
    /// <param name="parentIdSelector">Function extracting item's parent_id</param>
    /// <param name="rootId">Root element id</param>
    ///
    /// <exception cref="InvalidOperationException"></exception>
    /// <returns>Tree of items</returns>
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
            var message = "The collection must have exactly one root element, but ";
            var rootItemCount
                = collectionArray.Count(i => EqualityComparer<TKey?>.Default.Equals(parentIdSelector(i), rootId));
            switch (rootItemCount)
            {
                case < 1:
                    message += "none was found";
                    break;
                case > 1:
                    message += $"{rootItemCount} were found";
                    break;
            }

            throw new InvalidOperationException(message, e);
        }
    }

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

    public static IEnumerable<T> Flatten<T>(this T parent, Func<T, IEnumerable<T>> childrenSelector)
        => Enumerable.Repeat(parent, 1).Flatten(childrenSelector);

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

[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Required by algorhythm")]
public class TreeNode<T>(T item)
{
    public T Item { get; } = item;

    public ICollection<TreeNode<T>>? Children { get; set; }
}
