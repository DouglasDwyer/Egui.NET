using System.Collections;
using System.Collections.Immutable;

namespace Egui;

public partial record struct Classes : IEnumerable<ClassName>
{
    /// <summary>
    /// Returns the classes, in order, as a span.
    /// </summary>
    public readonly ReadOnlySpan<ClassName> AsSlice() => _classes.IsDefault ? default : _classes.AsSpan();

    /// <summary>
    /// True if the class is present.
    /// </summary>
    public readonly bool HasClass(ClassName @class)
    {
        foreach (var existing in AsSlice())
        {
            if (existing == @class)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Adds the class if <paramref name="present"/>, removes it otherwise.<br/>
    ///
    /// A class that is already present is moved to the end, since order can matter.
    /// </summary>
    public void SetClass(ClassName @class, bool present)
    {
        var builder = (_classes.IsDefault ? ImmutableArray<ClassName>.Empty : _classes).ToBuilder();
        builder.RemoveAll(x => x == @class);
        if (present)
        {
            builder.Add(@class);
        }

        _classes = builder.ToImmutable();
    }

    /// <summary>
    /// Adds the class in-place.
    /// </summary>
    public void AddClass(ClassName @class) => SetClass(@class, true);

    /// <summary>
    /// Adds the class in-place if <paramref name="condition"/> is true. This never removes a class.
    /// </summary>
    public void AddClassIf(ClassName @class, bool condition)
    {
        if (condition)
        {
            SetClass(@class, true);
        }
    }

    /// <summary>
    /// Removes the class in-place.
    /// </summary>
    public void RemoveClass(ClassName @class) => SetClass(@class, false);

    /// <summary>
    /// Appends all the given classes at the end, deduplicating them.
    /// </summary>
    public void AddClasses(Classes classes)
    {
        foreach (var @class in classes)
        {
            SetClass(@class, true);
        }
    }

    /// <summary>
    /// Returns a copy with the class added.
    /// </summary>
    public readonly Classes WithClass(ClassName @class)
    {
        var result = this;
        result.AddClass(@class);
        return result;
    }

    /// <summary>
    /// Returns a copy with the class added if <paramref name="condition"/> is true.
    /// </summary>
    public readonly Classes WithClassIf(ClassName @class, bool condition)
    {
        var result = this;
        result.AddClassIf(@class, condition);
        return result;
    }

    /// <summary>
    /// Returns a copy with all the given classes appended, deduplicating them.
    /// </summary>
    public readonly Classes WithClasses(Classes classes)
    {
        var result = this;
        result.AddClasses(classes);
        return result;
    }

    /// <summary>
    /// True if both have the same classes in the same order.
    /// </summary>
    public readonly bool Equals(Classes other) => AsSlice().SequenceEqual(other.AsSlice());

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var @class in AsSlice())
        {
            hash.Add(@class);
        }

        return hash.ToHashCode();
    }

    /// <summary>
    /// Iterates over the classes, in order.
    /// </summary>
    public readonly IEnumerator<ClassName> GetEnumerator()
    {
        return ((IEnumerable<ClassName>)(_classes.IsDefault ? ImmutableArray<ClassName>.Empty : _classes)).GetEnumerator();
    }

    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
