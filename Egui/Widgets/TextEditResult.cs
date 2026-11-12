using System.Diagnostics.CodeAnalysis;

namespace Egui.Widgets;

/// <summary>
/// The result of <see cref="TextEdit.ShowReturningText"/>: the output of the text edit and its text.
/// </summary>
public ref struct TextEditResult
{
    /// <summary>
    /// The output of the text edit.
    /// </summary>
    public TextEditOutput Output;

    private readonly bool _mutable;
    private ref string _text;
    private string _immutableText;

#pragma warning disable CS8618
    internal TextEditResult(TextEditOutput output, ref string text)
    {
        Output = output;
        _mutable = true;
        _text = ref text;
        _immutableText = "";
    }

    internal TextEditResult(TextEditOutput output, string immutableText)
    {
        Output = output;
        _mutable = false;
        _immutableText = immutableText;
    }
#pragma warning restore

    /// <summary>
    /// The text of the text edit. This is the string the text edit was created with,
    /// unless the text edit was created from an immutable string, in which case it is a copy.
    /// </summary>
    [UnscopedRef]
    public ref string Text
    {
        get
        {
            if (_mutable)
            {
                return ref _text;
            }

            return ref _immutableText;
        }
    }
}
