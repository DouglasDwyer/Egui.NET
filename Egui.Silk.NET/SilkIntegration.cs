using Egui.Viewport;
using System.Diagnostics;
using Silk.NET.Input;
using Silk.NET.Windowing;
using System.Collections.Immutable;
using System.Numerics;

using EguiKey = Egui.Key;
using SilkKey = Silk.NET.Input.Key;
using System.Runtime.InteropServices;

namespace Egui.Silk.NET;

/// <summary>
/// Base class for integration with <c>Silk.NET</c> windows.
/// This implementation handles collecting user input and applying
/// platform output, but not graphics. It must be derived with the
/// <see cref="DrawOutput"/> method overridden to provide rendering logic.
/// </summary>
public abstract class SilkIntegration : IDisposable
{
    /// <summary>
    /// Gets the state of the special keyboard keys.
    /// </summary>
    private Modifiers KeyModifiers => _leftModifiers.Plus(_rightModifiers);

    /// <summary>
    /// The context associated with this integration.
    /// </summary>
    public readonly Context EguiContext;

    /// <summary>
    /// The window to which rendering will occur.
    /// </summary>
    public readonly IWindow Window;

    /// <summary>
    /// Whether the window currently has OS focus.
    /// </summary>
    private bool _focused;

    /// <summary>
    /// Tracks the state of special keyboard keys on the left-hand side.
    /// </summary>
    private Modifiers _leftModifiers;

    /// <summary>
    /// The input to pass to <c>Egui</c> next frame.
    /// </summary>
    public RawInput _rawInput;

    /// <summary>
    /// Tracks the state of special keyboard keys on the right-hand side.
    /// </summary>
    private Modifiers _rightModifiers;

    /// <summary>
    /// Tracks the total time that <c>Egui</c> has been running.
    /// </summary>
    private readonly Stopwatch _timer;

    /// <summary>
    /// Creates a new integration object.
    /// </summary>
    /// <param name="context">The context to be displayed.</param>
    /// <param name="window">The window on which to draw.</param>
    public SilkIntegration(Context context, IWindow window)
    {
        EguiContext = context;
        _focused = false;
        _leftModifiers = new Modifiers();
        _rawInput = new RawInput();
        _rightModifiers = new Modifiers();
        _timer = new Stopwatch();
        Window = window;

        var input = window.CreateInput();

        for (int i = 0; i < input.Keyboards.Count; i++)
        {
            input.Keyboards[i].KeyDown += KeyDown;
            input.Keyboards[i].KeyChar += KeyChar;
            input.Keyboards[i].KeyUp += KeyUp;
        }
        for (int i = 0; i < input.Mice.Count; i++)
        {
            input.Mice[i].MouseMove += MouseMove;
            input.Mice[i].MouseDown += MouseDown;
            input.Mice[i].MouseUp += MouseUp;
        }

        Window.FocusChanged += x => _focused = x;
        _timer.Start();
    }

    /// <inheritdoc/>
    public virtual void Dispose() {}

    /// <summary>
    /// Run the UI code for one frame. Then, renders the screen
    /// and handles platform output.
    /// </summary>
    /// <param name="contextAction">All <see cref="Ui"/>-drawing code should execute in this closure.</param>
    public void Run(Action<Context> contextAction)
    {
        _rawInput.ViewportId = ViewportId.Root;
        _rawInput.Focused = _focused;
        _rawInput.Time = _timer.Elapsed.TotalSeconds;
        _rawInput.ScreenRect = Rect.FromMinSize(EPos2.Zero, (Window.Size.X, Window.Size.Y));

        _rawInput.Viewports = _rawInput.Viewports.SetItem(_rawInput.ViewportId, new ViewportInfo
        {
            Parent = null,
            Title = Window.Title,
            Events = ImmutableArray<ViewportEvent>.Empty,
            NativePixelsPerPoint = GetScaleFactor(),
            MonitorSize = null,
            Focused = _focused,
            InnerRect = _rawInput.ScreenRect
        });

        var output = EguiContext.Run(_rawInput, contextAction);
        DrawOutput(in output);
        _rawInput = new RawInput();
    }

    /// <summary>
    /// Renders the provided output to the screen using a native graphics API.
    /// </summary>
    /// <param name="output">The <c>Egui</c> data to draw.</param>
    protected abstract void DrawOutput(in FullOutput output);

    /// <summary>
    /// Converts from a <c>Silk.NET</c> key to an <c>Egui</c> key. 
    /// </summary>
    /// <param name="key">The key to convert.</param>
    /// <returns>The converted key, or <c>null</c> if there was no valid conversion.</returns>
    private static EguiKey? SilkToEguiKey(SilkKey key)
    {
        switch (key)
        {
            case SilkKey.Number1: return EguiKey.Num1;
            case SilkKey.Number2: return EguiKey.Num2;
            case SilkKey.Number3: return EguiKey.Num3;
            case SilkKey.Number4: return EguiKey.Num4;
            case SilkKey.Number5: return EguiKey.Num5;
            case SilkKey.Number6: return EguiKey.Num6;
            case SilkKey.Number7: return EguiKey.Num7;
            case SilkKey.Number8: return EguiKey.Num8;
            case SilkKey.Number9: return EguiKey.Num9;
            case SilkKey.Number0: return EguiKey.Num0;
            case SilkKey.Minus: return EguiKey.Minus;
            case SilkKey.Equal: return EguiKey.Equals;
            case SilkKey.Backspace: return EguiKey.Backspace;
            case SilkKey.GraveAccent: return EguiKey.Backtick;
            case SilkKey.Tab: return EguiKey.Tab;
            case SilkKey.Q: return EguiKey.Q;
            case SilkKey.W: return EguiKey.W;
            case SilkKey.E: return EguiKey.E;
            case SilkKey.R: return EguiKey.R;
            case SilkKey.T: return EguiKey.T;
            case SilkKey.Y: return EguiKey.Y;
            case SilkKey.U: return EguiKey.U;
            case SilkKey.I: return EguiKey.I;
            case SilkKey.O: return EguiKey.O;
            case SilkKey.P: return EguiKey.P;
            case SilkKey.LeftBracket: return EguiKey.OpenBracket;
            case SilkKey.RightBracket: return EguiKey.CloseBracket;
            case SilkKey.BackSlash: return EguiKey.Backslash;
            case SilkKey.A: return EguiKey.A;
            case SilkKey.S: return EguiKey.S;
            case SilkKey.D: return EguiKey.D;
            case SilkKey.F: return EguiKey.F;
            case SilkKey.G: return EguiKey.G;
            case SilkKey.H: return EguiKey.H;
            case SilkKey.J: return EguiKey.J;
            case SilkKey.K: return EguiKey.K;
            case SilkKey.L: return EguiKey.L;
            case SilkKey.Semicolon: return EguiKey.Semicolon;
            case SilkKey.Apostrophe: return EguiKey.Quote;
            case SilkKey.Z: return EguiKey.Z;
            case SilkKey.X: return EguiKey.X;
            case SilkKey.C: return EguiKey.C;
            case SilkKey.V: return EguiKey.V;
            case SilkKey.B: return EguiKey.B;
            case SilkKey.N: return EguiKey.N;
            case SilkKey.M: return EguiKey.M;
            case SilkKey.Comma: return EguiKey.Comma;
            case SilkKey.Period: return EguiKey.Period;
            case SilkKey.Slash: return EguiKey.Slash;
            case SilkKey.Space: return EguiKey.Space;
            case SilkKey.Enter: return EguiKey.Enter;
            case SilkKey.Escape: return EguiKey.Escape;
            case SilkKey.F1: return EguiKey.F1;
            case SilkKey.F2: return EguiKey.F2;
            case SilkKey.F3: return EguiKey.F3;
            case SilkKey.F4: return EguiKey.F4;
            case SilkKey.F5: return EguiKey.F5;
            case SilkKey.F6: return EguiKey.F6;
            case SilkKey.F7: return EguiKey.F7;
            case SilkKey.F8: return EguiKey.F8;
            case SilkKey.F9: return EguiKey.F9;
            case SilkKey.F10: return EguiKey.F10;
            case SilkKey.F11: return EguiKey.F11;
            case SilkKey.F12: return EguiKey.F12;
            case SilkKey.End: return EguiKey.End;
            case SilkKey.Delete: return EguiKey.Delete;
            case SilkKey.Left: return EguiKey.ArrowLeft;
            case SilkKey.Right: return EguiKey.ArrowRight;
            case SilkKey.Up: return EguiKey.ArrowUp;
            case SilkKey.Down: return EguiKey.ArrowDown;
        }

        return null;
    }

    /// <summary>
    /// Handles the beginning of a keypress.
    /// </summary>
    /// <param name="keyboard">The keyboard object.</param>
    /// <param name="key">The key that was pressed.</param>
    /// <param name="keyCode">The physical key code.</param>
    private void KeyDown(IKeyboard keyboard, SilkKey key, int keyCode)
    {
        ModifierKeyChange(key, true);

        var modifiers = KeyModifiers;

        if (modifiers.Ctrl && key == SilkKey.C)
        {
            _rawInput.Events = _rawInput.Events.Add(new Event.Copy());
            return;
        }

        if (modifiers.Ctrl && key == SilkKey.V)
        {
            if (keyboard.ClipboardText.Length > 0)
            {
                _rawInput.Events = _rawInput.Events.Add(new Event.Paste() { Value = keyboard.ClipboardText });
            }
            return;
        }

        if (modifiers.Ctrl && key == SilkKey.X)
        {
            _rawInput.Events = _rawInput.Events.Add(new Event.Cut());
            return;
        }

        var mapped = SilkToEguiKey(key);

        if (mapped.HasValue)
        {
            _rawInput.Events = _rawInput.Events.Add(new Event.Key
            {
                LogicalKey = mapped.Value,
                PhysicalKey = mapped.Value,
                Pressed = true,
                Modifiers = KeyModifiers
            });
        }
    }

    /// <summary>
    /// Handles a text input.
    /// </summary>
    /// <param name="keyboard">The keyboard object.</param>
    /// <param name="data">The character that was entered using the keyboard.</param>
    private void KeyChar(IKeyboard keyboard, char data)
    {
        _rawInput.Events = _rawInput.Events.Add(new Event.Text(data.ToString()));
    }

    /// <summary>
    /// Handles the end of a keypress.
    /// </summary>
    /// <param name="keyboard">The keyboard object.</param>
    /// <param name="key">The key that was pressed.</param>
    /// <param name="keyCode">The physical key code.</param>
    private void KeyUp(IKeyboard keyboard, SilkKey key, int keyCode)
    {
        ModifierKeyChange(key, false);

        var mapped = SilkToEguiKey(key);

        if (mapped.HasValue)
        {
            _rawInput.Events = _rawInput.Events.Add(new Event.Key
            {
                LogicalKey = mapped.Value,
                PhysicalKey = mapped.Value,
                Pressed = false,
                Modifiers = KeyModifiers
            });
        }
    }

    /// <summary>
    /// Handles a change in the mouse position.
    /// </summary>
    /// <param name="mouse">The mouse object.</param>
    /// <param name="vector">The change in position, in pixels.</param>
    private void MouseMove(IMouse mouse, Vector2 vector)
    {
        _rawInput.Events = _rawInput.Events.Add(new Event.PointerMoved
        {
            Value = new(vector.X, vector.Y)
        });
    }

    /// <summary>
    /// Handles the beginning of a mouse click.
    /// </summary>
    /// <param name="mouse">The mouse object.</param>
    /// <param name="button">The button that was pressed.</param>
    private void MouseDown(IMouse mouse, MouseButton button)
    {
        _rawInput.Events = _rawInput.Events.Add(new Event.PointerButton
        {
            Button = (PointerButton)button,
            Pressed = true,
            Pos = (mouse.Position.X, mouse.Position.Y),
            Modifiers = KeyModifiers
        });
    }

    /// <summary>
    /// Handles the end of a mouse click.
    /// </summary>
    /// <param name="mouse">The mouse object.</param>
    /// <param name="button">The button that was pressed.</param>
    private void MouseUp(IMouse mouse, MouseButton button)
    {
        _rawInput.Events = _rawInput.Events.Add(new Event.PointerButton
        {
            Button = (PointerButton)button,
            Pressed = false,
            Pos = (mouse.Position.X, mouse.Position.Y),
            Modifiers = KeyModifiers
        });
    }

    /// <summary>
    /// Checks to see whether <paramref name="key"/> corresponds to a special key.
    /// If so, updates the <see cref="_leftModifiers"/> and <see cref="_rightModifiers"/> accordingly.  
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    private void ModifierKeyChange(SilkKey key, bool value)
    {
        switch (key)
        {
            case SilkKey.AltLeft:
                _leftModifiers.Alt = value;
                break;
            case SilkKey.ControlLeft:
                _leftModifiers.Ctrl = value;
                break;
            case SilkKey.ShiftLeft:
                _leftModifiers.Shift = value;
                break;
            case SilkKey.AltRight:
                _rightModifiers.Alt = value;
                break;
            case SilkKey.ControlRight:
                _rightModifiers.Ctrl = value;
                break;
            case SilkKey.ShiftRight:
                _rightModifiers.Shift = value;
                break;
        }
    }

    /// <summary>
    /// Gets the high-DPI value associated with the provided window.
    /// </summary>
    /// <returns>The scale factor to use.</returns>
    private float GetScaleFactor()
    {
        if (Window.Native?.Win32 is (var hWnd, _, _))
        {
            return GetDpiForWindow(hWnd) / 96.0f;
        }
        else
        {
            return (float)Window.FramebufferSize.X / Window.Size.X;
        }
    }

    /// <summary>
    /// On Windows, gets the DPI associated with the provided window handle.
    /// </summary>
    /// <param name="hWnd">The raw window handle.</param>
    /// <returns>The associated DPI.</returns>
    [DllImport("User32.dll")]
    private static extern uint GetDpiForWindow(nint hWnd);
}