namespace System.Runtime.CompilerServices;

/// <summary>
/// Polyfill required to use C# 9 <c>init</c> accessors (and therefore <c>record</c>/
/// <c>record struct</c> types) when targeting <c>netstandard2.0</c>, which doesn't ship this
/// marker type itself.
/// </summary>
internal static class IsExternalInit;
