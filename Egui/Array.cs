using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Egui;

/// <summary>
/// A stack-allocated array type with two elements.
/// </summary>
/// <typeparam name="T">
/// The type of element in the array.
/// </typeparam>
[InlineArray(2)]
public struct Array2<T> : IArray<T>, IEnumerable<T>
{
    /// <summary>
    /// The number of elements in the array.
    /// </summary>
    public static int Length => 2;

    /// <summary>
    /// The element type of the array.
    /// </summary>
    private T _element0;

    /// <summary>
    /// Initializes an array with the given contents.
    /// </summary>
    /// <param name="e0">The <c>0</c>-index element.</param>
    /// <param name="e1">The <c>1</c>-index element.</param>
    public Array2(T e0, T e1)
    {
        _element0 = e0;
        this[1] = e1;
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => new ArrayEnumerator<T, Array2<T>>(this);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Array2<T> rhs)
        {
            return this == rhs;
        }
        else
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var result = 0;
        for (var i = 0; i < Length; i++)
        {
            result ^= this[i]?.GetHashCode() ?? 0;
        }
        return result;
    }

    /// <summary>
    /// Compares two objects for equality.
    /// </summary>
    /// <param name="lhs">The first object.</param>
    /// <param name="rhs">The second object.</param>
    /// <returns><c>true</c> when the objects are the same.</returns>
    public static bool operator ==(Array2<T> lhs, Array2<T> rhs)
    {
        for (var i = 0; i < Length; i++)
        {
            if (!Equals(lhs[i], rhs[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Compares two objects for inequality.
    /// </summary>
    /// <param name="lhs">The first object.</param>
    /// <param name="rhs">The second object.</param>
    /// <returns><c>true</c> when the objects are different.</returns>
    public static bool operator !=(Array2<T> lhs, Array2<T> rhs) => !(lhs == rhs);

    /// <summary>
    /// Converts a tuple to an array.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Array2<T>((T, T) value) => new Array2<T>(value.Item1, value.Item2);

    /// <summary>
    /// Serializes an instance of this type.
    /// </summary>
    internal static void Serialize(BincodeSerializer serializer, Array2<T> value) => value.Serialize(serializer);

    /// <summary>
    /// Serializes this object.
    /// </summary>
    internal void Serialize(BincodeSerializer serializer)
    {
        foreach (var item in this)
        {
            EguiMarshal.SerializerCache<T>.Serialize(serializer, item);
        }
    }

    /// <summary>
    /// Deserializes an instance of this type.
    /// </summary>
    internal static Array2<T> Deserialize(BincodeDeserializer deserializer)
    {
        Array2<T> result = default;
        for (var i = 0; i < Length; i++)
        {
            result[i] = EguiMarshal.SerializerCache<T>.Deserialize(deserializer);
        }
        return result;
    }

    /// <inheritdoc/>
    T IArray<T>.Get(int i) => this[i];
}

/// <summary>
/// A stack-allocated array type with three elements.
/// </summary>
/// <typeparam name="T">
/// The type of element in the array.
/// </typeparam>
[InlineArray(3)]
public struct Array3<T> : IArray<T>, IEnumerable<T>
{
    /// <summary>
    /// The number of elements in the array.
    /// </summary>
    public static int Length => 3;

    /// <summary>
    /// The element type of the array.
    /// </summary>
    private T _element0;

    /// <summary>
    /// Initializes an array with the given contents.
    /// </summary>
    /// <param name="e0">The <c>0</c>-index element.</param>
    /// <param name="e1">The <c>1</c>-index element.</param>
    /// <param name="e2">The <c>2</c>-index element.</param>
    public Array3(T e0, T e1, T e2)
    {
        _element0 = e0;
        this[1] = e1;
        this[2] = e2;
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => new ArrayEnumerator<T, Array3<T>>(this);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Array3<T> rhs)
        {
            return this == rhs;
        }
        else
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var result = 0;
        for (var i = 0; i < Length; i++)
        {
            result ^= this[i]?.GetHashCode() ?? 0;
        }
        return result;
    }

    /// <summary>
    /// Compares two objects for equality.
    /// </summary>
    /// <param name="lhs">The first object.</param>
    /// <param name="rhs">The second object.</param>
    /// <returns><c>true</c> when the objects are the same.</returns>
    public static bool operator ==(Array3<T> lhs, Array3<T> rhs)
    {
        for (var i = 0; i < Length; i++)
        {
            if (!Equals(lhs[i], rhs[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Compares two objects for inequality.
    /// </summary>
    /// <param name="lhs">The first object.</param>
    /// <param name="rhs">The second object.</param>
    /// <returns><c>true</c> when the objects are different.</returns>
    public static bool operator !=(Array3<T> lhs, Array3<T> rhs) => !(lhs == rhs);

    /// <summary>
    /// Converts a tuple to an array.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Array3<T>((T, T, T) value) => new Array3<T>(value.Item1, value.Item2, value.Item3);

    /// <summary>
    /// Serializes an instance of this type.
    /// </summary>
    internal static void Serialize(BincodeSerializer serializer, Array3<T> value) => value.Serialize(serializer);

    /// <summary>
    /// Serializes this object.
    /// </summary>
    internal void Serialize(BincodeSerializer serializer)
    {
        foreach (var item in this)
        {
            EguiMarshal.SerializerCache<T>.Serialize(serializer, item);
        }
    }

    /// <summary>
    /// Deserializes an instance of this type.
    /// </summary>
    internal static Array3<T> Deserialize(BincodeDeserializer deserializer)
    {
        Array3<T> result = default;
        for (var i = 0; i < Length; i++)
        {
            result[i] = EguiMarshal.SerializerCache<T>.Deserialize(deserializer);
        }
        return result;
    }

    /// <inheritdoc/>
    T IArray<T>.Get(int i) => this[i];
}

/// <summary>
/// A stack-allocated array type with two elements.
/// </summary>
/// <typeparam name="T">
/// The type of element in the array.
/// </typeparam>
[InlineArray(4)]
public struct Array4<T> : IArray<T>, IEnumerable<T>
{
    /// <summary>
    /// The number of elements in the array.
    /// </summary>
    public static int Length => 4;

    /// <summary>
    /// The element type of the array.
    /// </summary>
    private T _element0;

    /// <summary>
    /// Initializes an array with the given contents.
    /// </summary>
    /// <param name="e0">The <c>0</c>-index element.</param>
    /// <param name="e1">The <c>1</c>-index element.</param>
    /// <param name="e2">The <c>2</c>-index element.</param>
    /// <param name="e3">The <c>3</c>-index element.</param>
    public Array4(T e0, T e1, T e2, T e3)
    {
        _element0 = e0;
        this[1] = e1;
        this[2] = e2;
        this[3] = e3;
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => new ArrayEnumerator<T, Array4<T>>(this);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Array4<T> rhs)
        {
            return this == rhs;
        }
        else
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var result = 0;
        for (var i = 0; i < Length; i++)
        {
            result ^= this[i]?.GetHashCode() ?? 0;
        }
        return result;
    }

    /// <summary>
    /// Compares two objects for equality.
    /// </summary>
    /// <param name="lhs">The first object.</param>
    /// <param name="rhs">The second object.</param>
    /// <returns><c>true</c> when the objects are the same.</returns>
    public static bool operator ==(Array4<T> lhs, Array4<T> rhs)
    {
        for (var i = 0; i < Length; i++)
        {
            if (!Equals(lhs[i], rhs[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Compares two objects for inequality.
    /// </summary>
    /// <param name="lhs">The first object.</param>
    /// <param name="rhs">The second object.</param>
    /// <returns><c>true</c> when the objects are different.</returns>
    public static bool operator !=(Array4<T> lhs, Array4<T> rhs) => !(lhs == rhs);

    /// <summary>
    /// Converts a tuple to an array.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Array4<T>((T, T, T, T) value) => new Array4<T>(value.Item1, value.Item2, value.Item3, value.Item4);

    /// <summary>
    /// Serializes an instance of this type.
    /// </summary>
    internal static void Serialize(BincodeSerializer serializer, Array4<T> value) => value.Serialize(serializer);

    /// <summary>
    /// Serializes this object.
    /// </summary>
    internal void Serialize(BincodeSerializer serializer)
    {
        foreach (var item in this)
        {
            EguiMarshal.SerializerCache<T>.Serialize(serializer, item);
        }
    }

    /// <summary>
    /// Deserializes an instance of this type.
    /// </summary>
    internal static Array4<T> Deserialize(BincodeDeserializer deserializer)
    {
        Array4<T> result = default;
        for (var i = 0; i < Length; i++)
        {
            result[i] = EguiMarshal.SerializerCache<T>.Deserialize(deserializer);
        }
        return result;
    }

    /// <inheritdoc/>
    T IArray<T>.Get(int i) => this[i];
}

/// <summary>
/// A stack-allocated array type with five elements.
/// </summary>
/// <typeparam name="T">
/// The type of element in the array.
/// </typeparam>
[InlineArray(5)]
public struct Array5<T> : IArray<T>, IEnumerable<T>
{
    /// <summary>
    /// The number of elements in the array.
    /// </summary>
    public static int Length => 5;

    /// <summary>
    /// The element type of the array.
    /// </summary>
    private T _element0;

    /// <summary>
    /// Initializes an array with the given contents.
    /// </summary>
    /// <param name="e0">The <c>0</c>-index element.</param>
    /// <param name="e1">The <c>1</c>-index element.</param>
    /// <param name="e2">The <c>2</c>-index element.</param>
    /// <param name="e3">The <c>3</c>-index element.</param>
    /// <param name="e4">The <c>4</c>-index element.</param>
    public Array5(T e0, T e1, T e2, T e3, T e4)
    {
        _element0 = e0;
        this[1] = e1;
        this[2] = e2;
        this[3] = e3;
        this[4] = e4;
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => new ArrayEnumerator<T, Array5<T>>(this);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Array5<T> rhs)
        {
            return this == rhs;
        }
        else
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var result = 0;
        for (var i = 0; i < Length; i++)
        {
            result ^= this[i]?.GetHashCode() ?? 0;
        }
        return result;
    }

    /// <summary>
    /// Compares two objects for equality.
    /// </summary>
    /// <param name="lhs">The first object.</param>
    /// <param name="rhs">The second object.</param>
    /// <returns><c>true</c> when the objects are the same.</returns>
    public static bool operator ==(Array5<T> lhs, Array5<T> rhs)
    {
        for (var i = 0; i < Length; i++)
        {
            if (!Equals(lhs[i], rhs[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Compares two objects for inequality.
    /// </summary>
    /// <param name="lhs">The first object.</param>
    /// <param name="rhs">The second object.</param>
    /// <returns><c>true</c> when the objects are different.</returns>
    public static bool operator !=(Array5<T> lhs, Array5<T> rhs) => !(lhs == rhs);

    /// <summary>
    /// Converts a tuple to an array.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Array5<T>((T, T, T, T, T) value) => new Array5<T>(value.Item1, value.Item2, value.Item3, value.Item4, value.Item5);

    /// <summary>
    /// Serializes an instance of this type.
    /// </summary>
    internal static void Serialize(BincodeSerializer serializer, Array5<T> value) => value.Serialize(serializer);

    /// <summary>
    /// Serializes this object.
    /// </summary>
    internal void Serialize(BincodeSerializer serializer)
    {
        foreach (var item in this)
        {
            EguiMarshal.SerializerCache<T>.Serialize(serializer, item);
        }
    }

    /// <summary>
    /// Deserializes an instance of this type.
    /// </summary>
    internal static Array5<T> Deserialize(BincodeDeserializer deserializer)
    {
        Array5<T> result = default;
        for (var i = 0; i < Length; i++)
        {
            result[i] = EguiMarshal.SerializerCache<T>.Deserialize(deserializer);
        }
        return result;
    }

    /// <inheritdoc/>
    T IArray<T>.Get(int i) => this[i];
}

/// <summary>
/// A stack-allocated array type with six elements.
/// </summary>
/// <typeparam name="T">
/// The type of element in the array.
/// </typeparam>
[InlineArray(6)]
public struct Array6<T> : IArray<T>, IEnumerable<T>
{
    /// <summary>
    /// The number of elements in the array.
    /// </summary>
    public static int Length => 6;

    /// <summary>
    /// The element type of the array.
    /// </summary>
    private T _element0;

    /// <summary>
    /// Initializes an array with the given contents.
    /// </summary>
    /// <param name="e0">The <c>0</c>-index element.</param>
    /// <param name="e1">The <c>1</c>-index element.</param>
    /// <param name="e2">The <c>2</c>-index element.</param>
    /// <param name="e3">The <c>3</c>-index element.</param>
    /// <param name="e4">The <c>4</c>-index element.</param>
    /// <param name="e5">The <c>5</c>-index element.</param>
    public Array6(T e0, T e1, T e2, T e3, T e4, T e5)
    {
        _element0 = e0;
        this[1] = e1;
        this[2] = e2;
        this[3] = e3;
        this[4] = e4;
        this[5] = e5;
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => new ArrayEnumerator<T, Array6<T>>(this);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Array6<T> rhs)
        {
            return this == rhs;
        }
        else
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var result = 0;
        for (var i = 0; i < Length; i++)
        {
            result ^= this[i]?.GetHashCode() ?? 0;
        }
        return result;
    }

    /// <summary>
    /// Compares two objects for equality.
    /// </summary>
    /// <param name="lhs">The first object.</param>
    /// <param name="rhs">The second object.</param>
    /// <returns><c>true</c> when the objects are the same.</returns>
    public static bool operator ==(Array6<T> lhs, Array6<T> rhs)
    {
        for (var i = 0; i < Length; i++)
        {
            if (!Equals(lhs[i], rhs[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Compares two objects for inequality.
    /// </summary>
    /// <param name="lhs">The first object.</param>
    /// <param name="rhs">The second object.</param>
    /// <returns><c>true</c> when the objects are different.</returns>
    public static bool operator !=(Array6<T> lhs, Array6<T> rhs) => !(lhs == rhs);

    /// <summary>
    /// Converts a tuple to an array.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Array6<T>((T, T, T, T, T, T) value) => new Array6<T>(value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6);

    /// <summary>
    /// Serializes an instance of this type.
    /// </summary>
    internal static void Serialize(BincodeSerializer serializer, Array6<T> value) => value.Serialize(serializer);

    /// <summary>
    /// Serializes this object.
    /// </summary>
    internal void Serialize(BincodeSerializer serializer)
    {
        foreach (var item in this)
        {
            EguiMarshal.SerializerCache<T>.Serialize(serializer, item);
        }
    }

    /// <summary>
    /// Deserializes an instance of this type.
    /// </summary>
    internal static Array6<T> Deserialize(BincodeDeserializer deserializer)
    {
        Array6<T> result = default;
        for (var i = 0; i < Length; i++)
        {
            result[i] = EguiMarshal.SerializerCache<T>.Deserialize(deserializer);
        }
        return result;
    }

    /// <inheritdoc/>
    T IArray<T>.Get(int i) => this[i];
}

/// <summary>
/// Marks an array type.
/// </summary>
/// <typeparam name="T">
/// The element of the array.
/// </typeparam>
internal interface IArray<T>
{
    /// <summary>
    /// The number of elements in the array.
    /// </summary>
    static abstract int Length { get; }

    /// <summary>
    /// Gets the value at the given index.
    /// </summary>
    /// <param name="i">
    /// The index to lookup.
    /// </param>
    /// <returns>
    /// The value stored at the index.
    /// </returns>
    T Get(int i);
}

/// <summary>
/// Facilitates efficient iteration of array types.
/// </summary>
/// <typeparam name="T">The array element type.</typeparam>
/// <typeparam name="A">The backing array type.</typeparam>
struct ArrayEnumerator<T, A> : IEnumerator<T> where A : IArray<T>
{
    /// <summary>
    /// The underlying array object.
    /// </summary>
    private readonly A _array;

    /// <summary>
    /// The current array index.
    /// </summary>
    private int _index;

    /// <inheritdoc/>
    public T Current
    {
        get
        {
            if (_index < A.Length)
            {
                return _array.Get(_index);
            }
            else
            {
                throw new InvalidOperationException("Enumerator moved past end of the collection");
            }
        }
    }

    /// <inheritdoc/>
    object IEnumerator.Current => Current!;

    /// <summary>
    /// Creates a new enumerator.
    /// </summary>
    /// <param name="array">
    /// The array to iterate.
    /// </param>
    public ArrayEnumerator(A array)
    {
        _array = array;
        _index = 0;
    }

    /// <inheritdoc/>
    public void Dispose() { }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        _index++;
        return _index < A.Length;
    }

    /// <inheritdoc/>
    public void Reset()
    {
        _index = 0;
    }
}