using System.ComponentModel;

namespace Egui;

/// <summary>
/// A list of <see cref="Ui"/> objects. 
/// </summary>
public unsafe readonly ref struct UiArray
{
    /// <summary>
    /// The context associated with the <see cref="Ui"/>s. 
    /// </summary>
    private readonly Context _ctx;

    /// <summary>
    /// A pointer to the beginning of the handle list.
    /// </summary>
    private readonly nuint* _ptr;

    /// <summary>
    /// The number of UI objects.
    /// </summary>
    private readonly int _length;

    /// <summary>
    /// Whether the array has no elements.
    /// </summary>
    public bool IsEmpty => 0 == _length;

    /// <summary>
    /// The number of items in the array.
    /// </summary>
    public int Length => _length;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("'UiArray' does not contain a constructor that takes 0 arguments", error: true)]
    public UiArray() { throw new InvalidOperationException(); }

    /// <summary>
    /// Creates a new UI array.
    /// </summary>
    /// <param name="ctx">The context to use when creating UI objects.</param>
    /// <param name="ptr">A pointer to the underlying array.</param>
    /// <param name="length">The length of the array.</param>
    internal UiArray(Context ctx, nuint* ptr, int length)
    {
        _ctx = ctx;
        _ptr = ptr;
        _length = length;
    }

    /// <summary>
    /// Gets the UI with the provided index.
    /// </summary>
    /// <param name="index">The index to obtain.</param>
    /// <returns>The UI object.</returns>
    /// <exception cref="IndexOutOfRangeException">If <paramref name="index"/> was not within <see cref="Length"/>.</exception>
    public Ui this[int index]
    {
        get
        {
            if (index < _length)
            {
                return new Ui(_ctx, _ptr[index]);
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}