using System.ComponentModel;

namespace Egui.Util;

/// <summary>
/// Stores values identified by an <see cref="Id"/> AND the <see cref="Type"/> of the value.<br/>
/// In other words, it maps (Id, TypeId) to any value you want.
/// You can store state using the key <see cref="Id.Null"/>. The state will then only be identified by its type.
/// </summary>
public ref struct IdTypeMap
{
    /// <summary>
    /// Holds C#-side objects.
    /// </summary>
    private readonly Dictionary<RawKey, object> _inner;

    /// <summary>
    /// Whether this map contains no values.
    /// </summary>
    public bool IsEmpty => _inner.Count == 0;

    /// <summary>
    /// The number of values stored in this map.
    /// </summary>
    public int Length => _inner.Count;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("'IdTypeMap' does not contain a constructor that takes 0 arguments", error: true)]
    public IdTypeMap() { throw new InvalidOperationException(); }

    /// <summary>
    /// Creates a new map wrapper.
    /// </summary>
    /// <param name="inner">The inner map to modify.</param>
    internal IdTypeMap(Dictionary<RawKey, object> inner)
    {
        _inner = inner;
    }

    /// <summary>
    /// Removes all values from this map.
    /// </summary>
    public void Clear()
    {
        _inner.Clear();
    }

    /// <summary>
    /// Count the number of values are stored with the given type.
    /// </summary>
    public readonly int Count<T>()
    {
        return _inner.Keys.Count(x => x.Type == typeof(T));
    }

    /// <summary>
    /// Reads a value without trying to deserialize a persisted value.
    /// </summary>
    public readonly T? GetTemp<T>(Id id) where T : struct
    {
        return GetTempRaw(new RawKey(typeof(T), id)) is { } value ? (T)value : null;
    }

    /// <summary>
    /// Insert a value that will not be persisted.
    /// </summary>
    public void InsertTemp<T>(Id id, T value) where T : struct
    {
        InsertTempRaw(new RawKey(typeof(T), id), value);
    }

    /// <summary>
    /// Remove the state of this type and id.
    /// </summary>
    public void Remove<T>(Id id) where T : struct
    {
        RemoveTempRaw(new RawKey(typeof(T), id));
    }

    /// <summary>
    /// Note all state of the given type.
    /// </summary>
    public void RemoveByType<T>() where T : struct
    {
        foreach (var key in _inner.Keys.Where(x => x.Type == typeof(T)).ToList())
        {
            _inner.Remove(key);
        }
    }

    /// <summary>
    /// Remove and fetch the state of this type and id.
    /// </summary>
    public T? RemoveTemp<T>(Id id) where T : struct
    {
        return RemoveTempRaw(new RawKey(typeof(T), id)) is { } value ? (T)value : null;
    }

    /// <summary>
    /// Gets the value for a given raw key.
    /// </summary>
    public readonly object? GetTempRaw(RawKey raw)
    {
        return _inner.TryGetValue(raw, out var value) ? value : null;
    }

    /// <summary>
    /// Inserts (or replaces) the value for a given raw key.
    /// </summary>
    public void InsertTempRaw(RawKey raw, object value)
    {
        _inner[raw] = value;
    }

    /// <summary>
    /// Removes and returns the value for a given raw key.
    /// </summary>
    public object? RemoveTempRaw(RawKey raw)
    {
        return _inner.Remove(raw, out var value) ? value : null;
    }

    /// <summary>
    /// Returns all <see cref="RawKey"/>s to values in this map.
    /// </summary>
    public readonly IEnumerable<RawKey> TempKeys()
    {
        return _inner.Keys;
    }
}
