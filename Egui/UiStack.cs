#pragma warning disable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Collections;

namespace Egui;

/// <summary>
/// Information about a <c>Ui</c> and its parents.
///
/// <c>UiStack</c> serves to keep track of the current hierarchy of <c>Ui</c>s, such
/// that nested widgets or user code may adapt to the surrounding context or obtain layout information
/// from a <c>Ui</c> that might be several steps higher in the hierarchy.
///
/// Note: since <c>UiStack</c> contains a reference to its parent, it is both a stack, and a node within
/// that stack. Most of its methods are about the specific node, but some methods walk up the
/// hierarchy to provide information about the entire stack.
/// </summary>
public partial record struct UiStack : IEnumerable<UiStack> {
    public Id Id;
    public UiStackInfo Info;
    public Direction LayoutDirection;
    public Rect MinRect;
    public Rect MaxRect;
    public ReadOnlyBox<UiStack>? Parent;

    internal static void Serialize(BincodeSerializer serializer, UiStack value) => value.Serialize(serializer);

    internal void Serialize(BincodeSerializer serializer) {
        serializer.increase_container_depth();
        Id.Serialize(serializer);
        Info.Serialize(serializer);
        LayoutDirection.Serialize(serializer);
        MinRect.Serialize(serializer);
        MaxRect.Serialize(serializer);
        TraitHelpers.serialize_option_UiStack(Parent, serializer);
        serializer.decrease_container_depth();
    }

    internal static UiStack Deserialize(BincodeDeserializer deserializer) {
        deserializer.increase_container_depth();
        UiStack obj = default;
            obj.Id = Id.Deserialize(deserializer);
            obj.Info = UiStackInfo.Deserialize(deserializer);
            obj.LayoutDirection = DirectionSerdeExtensions.Deserialize(deserializer);
            obj.MinRect = Rect.Deserialize(deserializer);
            obj.MaxRect = Rect.Deserialize(deserializer);
            obj.Parent = TraitHelpers.deserialize_option_UiStack(deserializer);
            
        deserializer.decrease_container_depth();
        return obj;
    }
    /// <summary>
    /// Restricts the compiler-synthesized <see cref="ToString"/> to just this type's data
    /// fields, rather than every convenience property defined on this type. <see cref="Parent"/>
    /// is intentionally omitted so printing a deeply nested stack doesn't walk the whole chain.
    /// </summary>
    private readonly bool PrintMembers(StringBuilder builder)
    {
        builder.Append("Id = ").Append(Id);
        builder.Append(", Info = ").Append(Info);
        builder.Append(", LayoutDirection = ").Append(LayoutDirection);
        builder.Append(", MinRect = ").Append(MinRect);
        builder.Append(", MaxRect = ").Append(MaxRect);
        return true;
    }

    /// <inheritdoc/>
    public IEnumerator<UiStack> GetEnumerator()
    {
        var result = new List<UiStack>();
        var current = this;
        while (true)
        {
            result.Add(current);
            if (current.Parent is not null)
            {
                current = current.Parent.Value;
            }
        }
        return result.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}