using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Egui.Util;

namespace Egui;

/// <summary>
/// Your handle to egui.<br/>
///
/// This is the first thing you need when working with egui.
/// Contains the <see cref="InputState"/>, <see cref="Egui.Memory"/>, <see cref="PlatformOutput"/>, and more.
///
/// <see cref="Ui"/>  has many of the same accessor functions, and the same applies there.
/// </summary>
public sealed partial class Context : EguiObject
{
    /// <summary>
    /// A list of contexts that have been created.
    /// This list is used for deserializing the <see cref="Response.Ctx"/>
    /// field without allocating a new context object.
    /// </summary>
    private static readonly Dictionary<nuint, WeakReference<Context>> _contexts = new Dictionary<nuint, WeakReference<Context>>();

    /// <summary>
    /// Holds temporary data for widgets between frames.
    /// </summary>
    private readonly Dictionary<RawKey, object> _data = new Dictionary<RawKey, object>();

    /// <summary>
    /// Used to guard access to <see cref="_data"/>. 
    /// </summary>
    private readonly object _dataLocker = new object();

    /// <summary>
    /// Paint on top of everything else
    /// </summary>
    public Painter DebugPainter => new Painter(this, EguiMarshal.Call<nuint, EguiHandle>(EguiFn.egui_context_Context_debug_painter, Ptr));

    /// <summary>
    /// A unique ID used for internal tracking.
    /// </summary>
    internal readonly nuint Id;

    /// <summary>
    /// Returns the "default value" for a type.
    /// Default values are often some kind of initial value, identity value, or anything else that may make sense as a default.
    /// </summary>
    public Context() : base(EguiMarshal.Call<EguiHandle>(EguiFn.egui_context_Context_default))
    {
        lock (_contexts)
        {
            foreach (var pair in _contexts)
            {
                if (!pair.Value.TryGetTarget(out _))
                {
                    _contexts.Remove(pair.Key);
                }
            }

            Id = EguiMarshal.Call<nuint, nuint>(EguiFn.egui_context_Context_ref_id, Ptr);
            _contexts.Add(Id, new WeakReference<Context>(this));
        }
    }

    /// <summary>
    /// Retrieves an existing context.
    /// The result of this method is only defined if <see cref="ptr"/>
    /// is a pointer to a valid context previously created with the constructor.
    /// </summary>
    internal static Context FromId(nuint id)
    {
        lock (_contexts)
        {
            if (_contexts.TryGetValue(id, out var value)
                && value.TryGetTarget(out var target))
            {
                return target;
            }
        }

        throw new ArgumentException("Unable to find context with ID", nameof(id));
    }

    /// <summary>
    /// Mutate the <see cref="Style"/>s used by all subsequent windows, panels etc. in both dark and light mode.
    /// </summary>
    public void AllStylesMut(MutateDelegate<Style> mutateStyle)
    {
        StyleMutOf(Theme.Dark, mutateStyle);
        StyleMutOf(Theme.Light, mutateStyle);
    }

    /// <summary>
    /// Like <see cref="AnimateBool"/> but allows you to control the easing function.
    /// </summary>
    public float AnimateBoolWithEasing(Id id, bool value, Func<float, float> easing)
    {
        return AnimateBoolWithTimeAndEasing(id, value, GlobalStyle.AnimationTime, easing);
    }

    /// <summary>
    /// Like <see cref="AnimateBool"/> but allows you to control the animation time and easing function.<br/>
    ///
    /// Use e.g. <see cref="Emath.Easing.EasingHelpers.QuadraticOut"/>
    /// for a responsive start and a slow end.<br/>
    ///
    /// The easing function flips when <paramref name="targetValue"/> is <c>false</c>,
    /// so that when going back towards 0.0, we get
    /// </summary>
    public float AnimateBoolWithTimeAndEasing(Id id, bool targetValue, float animationTime, Func<float, float> easing)
    {
        var animatedValue = AnimateBoolWithTime(id, targetValue, GlobalStyle.AnimationTime);
        if (targetValue)
        {
            return easing(animatedValue);
        }
        else
        {
            return 1.0f - easing(1.0f - animatedValue);
        }
    }

    /// <summary>
    /// Read-write access to <see cref="IdTypeMap"/>, which stores superficial widget state.
    /// </summary>
    public void DataMut(Action<IdTypeMap> writer)
    {
        lock (_dataLocker)
        {
            writer(new IdTypeMap(_data));
        }
    }

    /// <inheritdoc cref="DataMut"/>
    public R DataMut<R>(Func<IdTypeMap, R> writer)
    {
        lock (_dataLocker)
        {
            return writer(new IdTypeMap(_data));
        }
    }

    /// <summary>
    /// Get a full-screen painter for a new or existing layer
    /// </summary>
    public Painter LayerPainter(LayerId layerId) => new Painter(this, EguiMarshal.Call<nuint, LayerId, EguiHandle>(EguiFn.egui_context_Context_layer_painter, Ptr, layerId));

    /// <summary>
    /// Run the ui code for one frame.<br/>
    /// At most <see cref="Options.MaxPasses"/>  calls will be issued to <paramref name="runUi"/>, and only on the rare occasion that <see cref="RequestDiscard"/> is called. Usually, it will only be called once.<br/>
    /// The <see cref="Ui"/> given to <paramref name="runUi"/> covers the entire viewport; put your widgets into a <see cref="Panel"/> , <see cref="CentralPanel"/> , <see cref="Window"/>  or <see cref="Area"/> .<br/>
    /// Instead of calling run, you can alternatively use <see cref="BeginPass"/>  and <see cref="EndPass"/> .<br/>
    /// </summary>
    public FullOutput Run(RawInput input, Action<Ui> runUi)
    {
        using var callback = new EguiCallback(ui => runUi(new Ui(this, ui)));
        return EguiMarshal.Call<nuint, RawInput, EguiCallback, FullOutput>(EguiFn.egui_context_Context_run_ui, Ptr, input, callback);
    }

    /// <summary>
    /// Like <see cref="Run(RawInput, Action{Ui})"/>, but reads input and
    /// writes output to an <see cref="EguiFfi"/> object.
    /// This may be used to pass data directly to a Rust-native integration
    /// library (like <c>egui-winit</c>) without a round-trip through C# data types.
    /// </summary>
    public unsafe void RunFfi(EguiFfi ffi, Action<Ui> runUi)
    {
        using var callback = new EguiCallback(ui => runUi(new Ui(this, ui)));
        EguiMarshal.Call(EguiFn.egui_context_Context_run_ffi, Ptr, ffi.Pointer, callback);
    }

    /// <summary>
    /// Run only the "logic" for one tick, without producing any ui or painting anything.<br/>
    ///
    /// Only the window state (<see cref="RawInput.Viewports"/> and <see cref="RawInput.Focused"/>)
    /// of <paramref name="input"/> is used, so that <paramref name="logic"/> can tell that the
    /// window is hidden. The ui input (events, time, …) is not interpreted, and is left for the
    /// next call to <see cref="Run"/>: <see cref="Input"/> is otherwise still that of the last pass.<br/>
    ///
    /// The returned <see cref="LogicOutput"/> is what <see cref="FullOutput"/> would have carried:
    /// anything <paramref name="logic"/> asked the integration to do. There is nothing to paint.
    /// </summary>
    public LogicOutput RunLogic(RawInput input, Action<Context> logic)
    {
        using var callback = new EguiCallback(_ => logic(this));
        return EguiMarshal.Call<nuint, RawInput, EguiCallback, LogicOutput>(EguiFn.egui_context_Context_run_logic, Ptr, input, callback);
    }

    /// <summary>
    /// Mutate the currently active <see cref="Egui.Style"/> used by all subsequent windows, panels etc. Use <see cref="AllStylesMut"/> to mutate both dark and light mode styles.
    /// </summary>
    /// <param name="mutateStyle"></param>
    public void StyleMut(MutateDelegate<Style> mutateStyle)
    {
        var style = GlobalStyle;
        try
        {
            mutateStyle(ref style);
        }
        finally
        {
            GlobalStyle = style;
        }
    }

    /// <summary>
    /// Mutate the <see cref="Style"/>  used by all subsequent windows, panels etc.
    /// </summary>
    public void StyleMutOf(Theme theme, MutateDelegate<Style> mutateStyle)
    {
        var style = StyleOf(theme);
        try
        {
            mutateStyle(ref style);
        }
        finally
        {
            SetStyleOf(theme, style);
        }
    }

    /// <summary>
    /// Read-write access to <see cref="FontsView"/>.<br/>
    /// Not valid until first call to <see cref="Run"/>. That’s because since we don’t know the proper <c>PixelsPerPoint</c> until then.
    /// </summary>
    public void Fonts(Action<FontsView> reader)
    {
        Fonts(f =>
        {
            reader(f);
            return false;
        });
    }

    /// <summary>
    /// Allocate a texture.<br/>
    /// This is for advanced users. Most users should use <see cref="Ui.Image"/> or <see cref="TryLoadTexture"/> instead.<br/>
    /// In order to display an image you must convert it to a texture using this function. The function will hand over the image data to the egui backend, which will upload it to the GPU.<br/>
    /// ⚠️ Make sure to only call this ONCE for each image, i.e. NOT in your main GUI code. The call is NOT immediate safe.<br/>
    /// The given name can be useful for later debugging, and will be visible if you call <see cref="TextureUi"/>.<br/>
    /// For how to load an image, see <see cref="ImageData"/> and <see cref="ColorImage.FromRgbaUnmultiplied"/>.
    /// See also <see cref="ImageData"/>, <see cref="Ui.Image"/> and <see cref="Image"/>.
    /// </summary>
    public TextureHandle LoadTexture(string name, ImageData image, TextureOptions options)
    {
        return new TextureHandle(EguiMarshal.Call<nuint, string, ImageData, TextureOptions, EguiHandle>(EguiFn.egui_context_Context_load_texture, Ptr, name, image, options));
    }

    /// <summary>
    /// Read-only access to <see cref="GraphicLayers"/>, where painted <see cref="Shape"/>s are written to.
    /// </summary>
    public void Graphics(Action<GraphicLayers> reader)
    {
        Graphics(i =>
        {
            reader(i);
            return false;
        });
    }

    /// <inheritdoc cref="Graphics"/>
    public R Graphics<R>(Func<GraphicLayers, R> reader)
    {
        var input = EguiMarshal.Call<nuint, GraphicLayers>(EguiFn.egui_context_Context_graphics, Ptr);
        return reader(input);
    }

    /// <summary>
    /// Read-write access to <see cref="GraphicLayers"/>, where painted <see cref="Shape"/>s are written to.
    /// </summary>
    public void GraphicsMut(MutateDelegate<GraphicLayers> writer)
    {
        GraphicsMut((ref GraphicLayers input) =>
        {
            writer(ref input);
            return false;
        });
    }

    /// <inheritdoc cref="GraphicsMut"/>
    public R GraphicsMut<R>(MutateDelegate<GraphicLayers, R> writer)
    {
        var graphics = Graphics(x => x);
        try
        {
            return writer(ref graphics);
        }
        finally
        {
            EguiMarshal.Call(EguiFn.egui_context_Context_graphics_mut, Ptr, graphics);
        }
    }

    /// <inheritdoc cref="Fonts"/>
    public R Fonts<R>(Func<FontsView, R> reader)
    {
        R result = default!;
        using var callback = new EguiCallback(f => result = reader(new FontsView(f)));
        EguiMarshal.Call(EguiFn.egui_context_Context_fonts_mut, Ptr, callback);
        return result;
    }

    /// <summary>
    /// Read-only access to <see cref="InputState"/>. 
    /// </summary>
    public void Input(Action<InputState> reader)
    {
        Input(i =>
        {
            reader(i);
            return false;
        });
    }

    /// <inheritdoc cref="Input"/>
    public R Input<R>(Func<InputState, R> reader)
    {
        var input = EguiMarshal.Call<nuint, InputState>(EguiFn.egui_context_Context_input, Ptr);
        return reader(input);
    }

    /// <summary>
    /// Read-write access to <see cref="InputState"/>. 
    /// </summary>
    public void InputMut(MutateDelegate<InputState> writer)
    {
        InputMut((ref InputState input) =>
        {
            writer(ref input);
            return false;
        });
    }

    /// <inheritdoc cref="InputMut"/>
    public R InputMut<R>(MutateDelegate<InputState, R> writer)
    {
        var input = Input(x => x);
        try
        {
            return writer(ref input);
        }
        finally
        {
            EguiMarshal.Call(EguiFn.egui_context_Context_input_mut, Ptr, input);
        }
    }

    /// <summary>
    /// Read-only access to <see cref="InputState"/>. 
    /// </summary>
    public void Options(Action<Options> reader)
    {
        Memory(m => reader(m.Options));
    }

    /// <inheritdoc cref="Options"/>
    public R Options<R>(Func<Options, R> reader)
    {
        return Memory(m => reader(m.Options));
    }

    /// <summary>
    /// Read-write access to <see cref="Options"/>. 
    /// </summary>
    public void OptionsMut(MutateDelegate<Options> writer)
    {
        MemoryMut(m =>
        {
            var opts = m.Options;
            writer(ref opts);
            m.Options = opts;
        });
    }

    /// <inheritdoc cref="OptionsMut"/>
    public R OptionsMut<R>(MutateDelegate<Options, R> writer)
    {
        return MemoryMut(m =>
        {
            var opts = m.Options;
            var result = writer(ref opts);
            m.Options = opts;
            return result;
        });
    }

    /// <summary>
    /// Read-only access to <see cref="Egui.Epaint.TessellationOptions"/>. 
    /// </summary>
    public void TessellationOptions(Action<TessellationOptions> reader)
    {
        TessellationOptions(i =>
        {
            reader(i);
            return false;
        });
    }

    /// <inheritdoc cref="TessellationOptions"/>
    public R TessellationOptions<R>(Func<TessellationOptions, R> reader)
    {
        var opts = EguiMarshal.Call<nuint, TessellationOptions>(EguiFn.egui_context_Context_tessellation_options, Ptr);
        return reader(opts);
    }

    /// <summary>
    /// Read-write access to <see cref="Egui.Epaint.TessellationOptions"/>. 
    /// </summary>
    public void TessellationOptionsMut(MutateDelegate<TessellationOptions> writer)
    {
        TessellationOptionsMut((ref TessellationOptions opts) =>
        {
            writer(ref opts);
            return false;
        });
    }

    /// <inheritdoc cref="TessellationOptionsMut"/>
    public R TessellationOptionsMut<R>(MutateDelegate<TessellationOptions, R> writer)
    {
        var opts = TessellationOptions(x => x);
        try
        {
            return writer(ref opts);
        }
        finally
        {
            EguiMarshal.Call(EguiFn.egui_context_Context_tessellation_options_mut, Ptr, opts);
        }
    }

    /// <summary>
    /// This will create a <see cref="InputState"/> if there is no input state for that viewport
    /// </summary>
    public void InputFor(ViewportId id, Action<InputState> reader)
    {
        InputFor(id, i =>
        {
            reader(i);
            return false;
        });
    }

    /// <inheritdoc cref="InputFor"/>
    public R InputFor<R>(ViewportId id, Func<InputState, R> reader)
    {
        var input = EguiMarshal.Call<nuint, ViewportId, InputState>(EguiFn.egui_context_Context_input_for, Ptr, id);
        return reader(input);
    }

    /// <summary>
    /// This will create a <see cref="InputState"/> if there is no input state for that viewport
    /// </summary>
    public void InputMutFor(ViewportId id, MutateDelegate<InputState> writer)
    {
        InputMutFor(id, (ref InputState input) =>
        {
            writer(ref input);
            return false;
        });
    }

    /// <inheritdoc cref="InputMutFor"/>
    public R InputMutFor<R>(ViewportId id, MutateDelegate<InputState, R> writer)
    {
        var input = InputFor(id, x => x);
        try
        {
            return writer(ref input);
        }
        finally
        {
            EguiMarshal.Call(EguiFn.egui_context_Context_input_mut_for, Ptr, id, input);
        }
    }

    /// <summary>
    /// Read-only access to <see cref="Memory"/>.
    /// </summary>
    private void Memory(Action<Memory> writer)
    {
        Memory(m =>
        {
            writer(m);
            return false;
        });
    }

    /// <inheritdoc cref="Memory"/>
    private R Memory<R>(Func<Memory, R> writer)
    {
        R result = default!;
        using var callback = new EguiCallback(m => result = writer(new Memory(m)));
        EguiMarshal.Call(EguiFn.egui_context_Context_memory_mut, Ptr, callback);
        return result;
    }

    /// <summary>
    /// Read-write access to <see cref="Memory"/>. 
    /// </summary>
    public void MemoryMut(Action<Memory> writer)
    {
        MemoryMut(m =>
        {
            writer(m);
            return false;
        });
    }

    /// <inheritdoc cref="MemoryMut"/>
    public R MemoryMut<R>(Func<Memory, R> writer)
    {
        R result = default!;
        using var callback = new EguiCallback(m => result = writer(new Memory(m)));
        EguiMarshal.Call(EguiFn.egui_context_Context_memory_mut, Ptr, callback);
        return result;
    }

    /// <summary>
    /// Read-only access to <see cref="PlatformOutput"/>. 
    /// </summary>
    public void Output(Action<PlatformOutput> reader)
    {
        Output(i =>
        {
            reader(i);
            return false;
        });
    }

    /// <inheritdoc cref="Output"/>
    public R Output<R>(Func<PlatformOutput, R> reader)
    {
        var input = EguiMarshal.Call<nuint, PlatformOutput>(EguiFn.egui_context_Context_output, Ptr);
        return reader(input);
    }

    /// <summary>
    /// Read-write access to <see cref="PlatformOutput"/>. 
    /// </summary>
    public void OutputMut(MutateDelegate<PlatformOutput> writer)
    {
        OutputMut((ref PlatformOutput output) =>
        {
            writer(ref output);
            return false;
        });
    }

    /// <inheritdoc cref="OutputMut"/>
    public R OutputMut<R>(MutateDelegate<PlatformOutput, R> writer)
    {
        var input = Output(x => x);
        try
        {
            return writer(ref input);
        }
        finally
        {
            EguiMarshal.Call(EguiFn.egui_context_Context_output_mut, Ptr, input);
        }
    }

    /// <summary>
    /// This is called by <see cref="Response.WidgetInfo"/>, but can also be called directly.
    /// With some debug flags it will store the widget info in <see cref="WidgetRects"/> for later display.
    /// </summary>
    public void RegisterWidgetInfo(Id id, Func<WidgetInfo> makeInfo)
    {
#if DEBUG
        EguiMarshal.Call(EguiFn.egui_context_Context_register_widget_info, Ptr, id, makeInfo());
#else
        _ = (id, makeInfo);
#endif
    }
}