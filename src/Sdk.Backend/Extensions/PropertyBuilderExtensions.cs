using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sdk.Messaging;

namespace Sdk.Backend.Extensions;

public static class PropertyBuilderExtensions
{
    public static PropertyBuilder<Dictionary<TKey, TValue>> PersistAsJson<TKey, TValue>(
        this PropertyBuilder<Dictionary<TKey, TValue>> propertyBuilder) where TKey : notnull
        => propertyBuilder.HasConversion(
            im => JsonSerializer.Serialize(im, DefaultJsonSerializerSettings.Default),
            dic => (string.IsNullOrEmpty(dic) ? null : JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(dic, DefaultJsonSerializerSettings.Default))
                    ?? new Dictionary<TKey, TValue>(),
            CreateDictionaryComparer<TKey, TValue>());

    public static PropertyBuilder<List<T>> PersistAsJson<T>(this PropertyBuilder<List<T>> propertyBuilder)
        => propertyBuilder.HasConversion(
            im => JsonSerializer.Serialize(im, DefaultJsonSerializerSettings.Default),
            list => (string.IsNullOrEmpty(list) ? null : JsonSerializer.Deserialize<List<T>>(list, DefaultJsonSerializerSettings.Default))
                    ?? new List<T>(),
            CreateListComparer<T>());

    public static PropertyBuilder<HashSet<T>> PersistAsJson<T>(this PropertyBuilder<HashSet<T>> propertyBuilder)
        => propertyBuilder.HasConversion(
            im => JsonSerializer.Serialize(im, DefaultJsonSerializerSettings.Default),
            set => (string.IsNullOrEmpty(set) ? null : JsonSerializer.Deserialize<HashSet<T>>(set, DefaultJsonSerializerSettings.Default))
                    ?? new HashSet<T>(),
            CreateHashSetComparer<T>());

    private static ValueComparer<List<T>> CreateListComparer<T>()
        => new(
            (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v != null ? v.GetHashCode() : 0)),
            c => c.ToList());

    private static ValueComparer<HashSet<T>> CreateHashSetComparer<T>()
        => new(
            (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v != null ? v.GetHashCode() : 0)),
            c => new HashSet<T>(c));

    private static ValueComparer<Dictionary<TKey, TValue>> CreateDictionaryComparer<TKey, TValue>() where TKey : notnull
        => new(
            (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => new Dictionary<TKey, TValue>(c));
}
