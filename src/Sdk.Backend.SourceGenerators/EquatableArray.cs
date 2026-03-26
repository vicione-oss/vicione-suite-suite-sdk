using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sdk.Backend.SourceGenerators;

/// <summary>
/// A thin wrapper around an array that provides structural equality,
/// required for correct incremental generator caching.
/// </summary>
internal readonly struct EquatableArray<T>(T[] array) : IEquatable<EquatableArray<T>>, IReadOnlyList<T>
    where T : IEquatable<T>
{
    private readonly T[]? _array = array;

    public int Count => _array?.Length ?? 0;

    public bool Equals(EquatableArray<T> other)
    {
        if (_array is null && other._array is null)
            return true;
        if (_array is null || other._array is null)
            return false;
        if (_array.Length != other._array.Length)
            return false;

        for (var i = 0; i < _array.Length; i++)
        {
            if (!_array[i].Equals(other._array[i]))
                return false;
        }

        return true;
    }

    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

    public override int GetHashCode()
        => _array?.Aggregate(17, (current, item) => (current * 31) + item.GetHashCode()) ?? 0;

    public IEnumerator<T> GetEnumerator()
    {
        var array = _array ?? [];
        return ((IEnumerable<T>)array).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public T this[int index] => (_array ?? [])[index];
}
