using Egui;
using Egui.Epaint;
using Xunit;

namespace Egui.Tests;

/// <summary>
/// Tests the behavior of the C# union types generated for Rust enums.
/// </summary>
public class UnionTests
{
    [Fact]
    public void ConvertsFromCaseAndPatternMatches()
    {
        TextureId id = new TextureId.User(5);

        var description = id switch
        {
            TextureId.Managed managed => $"managed {managed.Value}",
            TextureId.User user => $"user {user.Value}",
        };

        Assert.Equal("user 5", description);
        Assert.True(id.HasValue);
        Assert.True(id.TryGetValue(out TextureId.User u));
        Assert.Equal(5UL, u.Value);
        Assert.False(id.TryGetValue(out TextureId.Managed _));
        Assert.IsType<TextureId.User>(id.Value);
    }

    [Fact]
    public void EqualityDistinguishesCases()
    {
        TextureId a = new TextureId.User(5);
        TextureId b = new TextureId.User(5);
        TextureId c = new TextureId.Managed(5);

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
        Assert.NotEqual(a, c);
        Assert.True(a == b);
        Assert.True(a != c);
    }

    [Fact]
    public void UninitializedUnionWithDefaultFallsBackToRustDefault()
    {
        TextureId uninitialized = default;
        TextureId expected = new TextureId();

        Assert.True(uninitialized.HasValue);
        Assert.Equal(expected, uninitialized);
        Assert.Equal(expected.GetHashCode(), uninitialized.GetHashCode());
        Assert.True(uninitialized.TryGetValue(out TextureId.Managed managed));
        Assert.Equal(0UL, managed.Value);
        Assert.Equal(expected.ToString(), uninitialized.ToString());
    }

    [Fact]
    public void UninitializedUnionWithDefaultSerializesAsDefault()
    {
        WidgetText uninitialized = default;

        Assert.IsType<WidgetText.Text>(uninitialized.Value);
        Assert.Equal("", uninitialized.RawText);
    }

    [Fact]
    public void UnionRoundTripsThroughRust()
    {
        WidgetText text = "hello";

        Assert.Equal("hello", text.RawText);
    }

    [Fact]
    public void UninitializedUnionWithoutDefaultThrowsOnRead()
    {
        Primitive uninitialized = default;

        Assert.Throws<InvalidOperationException>(() => { _ = uninitialized.Value; });
        Assert.Throws<InvalidOperationException>(() => { _ = uninitialized.HasValue; });
        Assert.Throws<InvalidOperationException>(() => { _ = uninitialized.TryGetValue(out Primitive.Mesh _); });
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = uninitialized switch
            {
                Primitive.Mesh => 1,
            };
        });
    }

    [Fact]
    public void UninitializedUnionWithoutDefaultStillSupportsEqualityAndToString()
    {
        Primitive a = default;
        Primitive b = default;

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
        Assert.Contains("uninitialized", a.ToString());
    }

    [Fact]
    public void UninitializedUnionWithoutDefaultThrowsOnSerialization()
    {
        Shape uninitialized = default;

        Assert.ThrowsAny<Exception>(() => { _ = uninitialized.VisualBoundingRect; });
    }
}
