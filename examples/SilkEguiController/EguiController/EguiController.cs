using System.Numerics;
using System.Diagnostics;
using System.Collections.Immutable;

using Egui.Epaint;
using Egui.Viewport;

using Silk.NET.Input;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;

namespace Egui;

unsafe public class EguiController
{
    // silk
    GL gl;
    IWindow window;

    // egui
    public Context eguiContext;
    public RawInput eguiInput = new RawInput();

    EguiRenderer eguiRenderer;

    Stopwatch timer = new Stopwatch();

    float displayScaleFactor => (float)window.FramebufferSize.X / window.Size.X;

    bool shift = false;
    bool alt = false;
    bool ctrl = false;
    bool focus = false;

    public EguiController(GL gl, IWindow window, IInputContext input)
    {
        this.gl = gl;
        this.window = window;
        
        eguiContext = new Context();
        eguiRenderer = new EguiRenderer(gl);
        timer.Start();

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

        window.FocusChanged += x => focus = x;
    }

    public void Render(Action<Context> contextAction)
    {
        eguiInput.ViewportId = ViewportId.Root;
        eguiInput.Focused = focus;
        eguiInput.Time = timer.Elapsed.TotalSeconds;
        eguiInput.ScreenRect = Rect.FromMinSize(EPos2.Zero, (window.Size.X, window.Size.Y));

        eguiInput.SystemTheme = Theme.Dark;
        eguiInput.Viewports = eguiInput.Viewports.SetItem(eguiInput.ViewportId, new ViewportInfo
        {
            Parent = null,
            Title = "egui test",
            Events = ImmutableArray<ViewportEvent>.Empty,
            NativePixelsPerPoint = displayScaleFactor,
            MonitorSize = null,
            Focused = focus,
            InnerRect = eguiInput.ScreenRect
        });

        FullOutput output = eguiContext.Run(eguiInput, contextAction);
        DrawOutput(in output);
        eguiInput = new RawInput();
    }

    public Egui.Key? SilkToEguiKey(Silk.NET.Input.Key key)
    {
        switch (key)
        {
            case Silk.NET.Input.Key.Number1: return Egui.Key.Num1;
            case Silk.NET.Input.Key.Number2: return Egui.Key.Num2;
            case Silk.NET.Input.Key.Number3: return Egui.Key.Num3;
            case Silk.NET.Input.Key.Number4: return Egui.Key.Num4;
            case Silk.NET.Input.Key.Number5: return Egui.Key.Num5;
            case Silk.NET.Input.Key.Number6: return Egui.Key.Num6;
            case Silk.NET.Input.Key.Number7: return Egui.Key.Num7;
            case Silk.NET.Input.Key.Number8: return Egui.Key.Num8;
            case Silk.NET.Input.Key.Number9: return Egui.Key.Num9;
            case Silk.NET.Input.Key.Number0: return Egui.Key.Num0;
            case Silk.NET.Input.Key.Minus: return Egui.Key.Minus;
            case Silk.NET.Input.Key.Equal: return Egui.Key.Equals;
            case Silk.NET.Input.Key.Backspace: return Egui.Key.Backspace;
            case Silk.NET.Input.Key.GraveAccent: return Egui.Key.Backtick;
            case Silk.NET.Input.Key.Tab: return Egui.Key.Tab;
            case Silk.NET.Input.Key.Q: return Egui.Key.Q;
            case Silk.NET.Input.Key.W: return Egui.Key.W;
            case Silk.NET.Input.Key.E: return Egui.Key.E;
            case Silk.NET.Input.Key.R: return Egui.Key.R;
            case Silk.NET.Input.Key.T: return Egui.Key.T;
            case Silk.NET.Input.Key.Y: return Egui.Key.Y;
            case Silk.NET.Input.Key.U: return Egui.Key.U;
            case Silk.NET.Input.Key.I: return Egui.Key.I;
            case Silk.NET.Input.Key.O: return Egui.Key.O;
            case Silk.NET.Input.Key.P: return Egui.Key.P;
            case Silk.NET.Input.Key.LeftBracket: return Egui.Key.OpenBracket;
            case Silk.NET.Input.Key.RightBracket: return Egui.Key.CloseBracket;
            case Silk.NET.Input.Key.BackSlash: return Egui.Key.Backslash;
            case Silk.NET.Input.Key.A: return Egui.Key.A;
            case Silk.NET.Input.Key.S: return Egui.Key.S;
            case Silk.NET.Input.Key.D: return Egui.Key.D;
            case Silk.NET.Input.Key.F: return Egui.Key.F;
            case Silk.NET.Input.Key.G: return Egui.Key.G;
            case Silk.NET.Input.Key.H: return Egui.Key.H;
            case Silk.NET.Input.Key.J: return Egui.Key.J;
            case Silk.NET.Input.Key.K: return Egui.Key.K;
            case Silk.NET.Input.Key.L: return Egui.Key.L;
            case Silk.NET.Input.Key.Semicolon: return Egui.Key.Semicolon;
            case Silk.NET.Input.Key.Apostrophe: return Egui.Key.Quote;
            case Silk.NET.Input.Key.Z: return Egui.Key.Z;
            case Silk.NET.Input.Key.X: return Egui.Key.X;
            case Silk.NET.Input.Key.C: return Egui.Key.C;
            case Silk.NET.Input.Key.V: return Egui.Key.V;
            case Silk.NET.Input.Key.B: return Egui.Key.B;
            case Silk.NET.Input.Key.N: return Egui.Key.N;
            case Silk.NET.Input.Key.M: return Egui.Key.M;
            case Silk.NET.Input.Key.Comma: return Egui.Key.Comma;
            case Silk.NET.Input.Key.Period: return Egui.Key.Period;
            case Silk.NET.Input.Key.Slash: return Egui.Key.Slash;
            case Silk.NET.Input.Key.Space: return Egui.Key.Space;
            case Silk.NET.Input.Key.Enter: return Egui.Key.Enter;
            case Silk.NET.Input.Key.Escape: return Egui.Key.Escape;
            case Silk.NET.Input.Key.F1: return Egui.Key.F1;
            case Silk.NET.Input.Key.F2: return Egui.Key.F2;
            case Silk.NET.Input.Key.F3: return Egui.Key.F3;
            case Silk.NET.Input.Key.F4: return Egui.Key.F4;
            case Silk.NET.Input.Key.F5: return Egui.Key.F5;
            case Silk.NET.Input.Key.F6: return Egui.Key.F6;
            case Silk.NET.Input.Key.F7: return Egui.Key.F7;
            case Silk.NET.Input.Key.F8: return Egui.Key.F8;
            case Silk.NET.Input.Key.F9: return Egui.Key.F9;
            case Silk.NET.Input.Key.F10: return Egui.Key.F10;
            case Silk.NET.Input.Key.F11: return Egui.Key.F11;
            case Silk.NET.Input.Key.F12: return Egui.Key.F12;
            case Silk.NET.Input.Key.End: return Egui.Key.End;
            case Silk.NET.Input.Key.Delete: return Egui.Key.Delete;
            case Silk.NET.Input.Key.Left: return Egui.Key.ArrowLeft;
            case Silk.NET.Input.Key.Right: return Egui.Key.ArrowRight;
            case Silk.NET.Input.Key.Up: return Egui.Key.ArrowUp;
            case Silk.NET.Input.Key.Down: return Egui.Key.ArrowDown;
        }

        return null;
    }

    void KeyDown(IKeyboard keyboard, Silk.NET.Input.Key key, int keyCode)
    {
        if (key == Silk.NET.Input.Key.ShiftLeft || key == Silk.NET.Input.Key.ShiftRight) shift = true;
        if (key == Silk.NET.Input.Key.AltLeft || key == Silk.NET.Input.Key.AltRight) alt = true;
        if (key == Silk.NET.Input.Key.ControlLeft || key == Silk.NET.Input.Key.ControlRight) ctrl = true;

        if (ctrl && key == Silk.NET.Input.Key.C)
        {
            eguiInput.Events = eguiInput.Events.Add(new Event.Copy());
            return;
        }

        if (ctrl && key == Silk.NET.Input.Key.V)
        {
            if (keyboard.ClipboardText.Length > 0)
            {
                eguiInput.Events = eguiInput.Events.Add(new Event.Paste() { Value = keyboard.ClipboardText });
            }
            return;
        }

        if (ctrl && key == Silk.NET.Input.Key.X)
        {
            eguiInput.Events = eguiInput.Events.Add(new Event.Cut());
            return;
        }

        var mapped = SilkToEguiKey(key);

        if (mapped.HasValue)
        {
            eguiInput.Events = eguiInput.Events.Add(new Event.Key
            {
                LogicalKey = mapped.Value,
                PhysicalKey = mapped.Value,
                Pressed = true,
                Modifiers = new()
                {
                    Alt = alt,
                    Ctrl = ctrl,
                    Shift = shift
                }
            });
        }
    }

    void KeyChar(IKeyboard keyboard, char data)
    {
        eguiInput.Events = eguiInput.Events.Add(new Event.Text(data.ToString()));
    }

    void KeyUp(IKeyboard keyboard, Silk.NET.Input.Key key, int keyCode)
    {
        if (key == Silk.NET.Input.Key.ShiftLeft || key == Silk.NET.Input.Key.ShiftRight) shift = false;
        if (key == Silk.NET.Input.Key.AltLeft || key == Silk.NET.Input.Key.AltRight) alt = false;
        if (key == Silk.NET.Input.Key.ControlLeft || key == Silk.NET.Input.Key.ControlRight) ctrl = false;

        var mapped = SilkToEguiKey(key);

        if (mapped.HasValue)
        {
            eguiInput.Events = eguiInput.Events.Add(new Event.Key
            {
                LogicalKey = mapped.Value,
                PhysicalKey = mapped.Value,
                Pressed = false,
                Modifiers = new()
                {
                    Alt = alt,
                    Ctrl = ctrl,
                    Shift = shift
                }
            });
        }
    }

    void MouseMove(IMouse mouse, Vector2 vector)
    {
        eguiInput.Events = eguiInput.Events.Add(new Event.PointerMoved
        {
            Value = new(vector.X, vector.Y)
        });
    }

    void MouseDown(IMouse mouse, MouseButton button)
    {
        eguiInput.Events = eguiInput.Events.Add(new Event.PointerButton
        {
            Button = (PointerButton)button,
            Pressed = true,
            Pos = (mouse.Position.X, mouse.Position.Y),
            Modifiers = new()
            {
                Alt = alt,
                Ctrl = ctrl,
                Shift = shift
            }
        });
    }

    void MouseUp(IMouse mouse, MouseButton button)
    {
        eguiInput.Events = eguiInput.Events.Add(new Event.PointerButton
        {
            Button = (PointerButton)button,
            Pressed = false,
            Pos = (mouse.Position.X, mouse.Position.Y),
            Modifiers = new()
            {
                Alt = alt,
                Ctrl = ctrl,
                Shift = shift
            }
        });
    }


    void DrawOutput(in FullOutput output)
    {
        gl.Disable(GLEnum.ScissorTest);
        gl.Viewport(0, 0, (uint)window.FramebufferSize.X, (uint)window.FramebufferSize.Y);
        var clippedPrimitives = eguiContext.Tessellate(output.Shapes.ToImmutableArray(), output.PixelsPerPoint);
        var clippedPrimitivesReadOnly = clippedPrimitives.ToArray();
        eguiRenderer.RenderAndUpdateTextures((uint)window.FramebufferSize.X, (uint)window.FramebufferSize.Y, output.PixelsPerPoint, clippedPrimitivesReadOnly, output.TexturesDelta);
        gl.Disable(GLEnum.ScissorTest);
    }
}

unsafe public class EguiRenderer
{
    GL gl;

    uint shaderProgram;

    int uScreenSize;
    int uSampler;

    int aPosLoc;
    int aTcLoc;
    int aSrgbaLoc;

    uint vao;
    uint vbo;
    uint eao;

    Dictionary<TextureId, uint> textures;

    public EguiRenderer(GL gl)
    {
        this.gl = gl;
        
        CheckGlErrors();

        var frag = CompileShader(GLEnum.FragmentShader, FragShader());
        var vert = CompileShader(GLEnum.VertexShader, VertShader());
        shaderProgram = LinkProgram([vert, frag]);
        uScreenSize = this.gl.GetUniformLocation(shaderProgram, "u_screen_size");
        uSampler = this.gl.GetUniformLocation(shaderProgram, "u_sampler");

        vbo = this.gl.GenBuffer();

        CheckGlErrors();

        aPosLoc = this.gl.GetAttribLocation(shaderProgram, "a_pos");
        aTcLoc = this.gl.GetAttribLocation(shaderProgram, "a_tc");
        aSrgbaLoc = this.gl.GetAttribLocation(shaderProgram, "a_srgba");

        CheckGlErrors();

        vao = this.gl.GenVertexArray();
        CheckGlErrors();
        this.gl.BindVertexArray(vao);
        this.gl.BindBuffer(GLEnum.ArrayBuffer, vbo);
        CheckGlErrors();

        this.gl.EnableVertexAttribArray((uint)aPosLoc);
        this.gl.VertexAttribPointer((uint)aPosLoc, 2, GLEnum.Float, false, (uint)sizeof(Vertex), 0);
        CheckGlErrors();
        this.gl.EnableVertexAttribArray((uint)aTcLoc);
        CheckGlErrors();
        this.gl.VertexAttribPointer((uint)aTcLoc, 2, GLEnum.Float, false, (uint)sizeof(Vertex), 2 * 4);
        CheckGlErrors();
        this.gl.EnableVertexAttribArray((uint)aSrgbaLoc);
        this.gl.VertexAttribPointer((uint)aSrgbaLoc, 4, GLEnum.UnsignedByte, false, (uint)sizeof(Vertex), 4 * 4);

        CheckGlErrors();

        eao = this.gl.GenBuffer();

        textures = [];
        CheckGlErrors();
    }

    public void RenderAndUpdateTextures(uint width, uint height, float pixelsPerPoint, ReadOnlyMemory<ClippedPrimitive> clippedPrimitives, TexturesDelta texturesDelta)
    {
        CheckGlErrors();
        foreach (var (id, delta) in texturesDelta.Set) SetTexture(id, delta);
        CheckGlErrors();
        RenderPrimitives(width, height, pixelsPerPoint, clippedPrimitives);
        CheckGlErrors();
        foreach (var id in texturesDelta.Free) FreeTexture(id);
        CheckGlErrors();
    }

    void PrepareRendering(uint width, uint height, float pixelsPerPoint)
    {
        gl.Enable(GLEnum.ScissorTest);
        gl.Disable(GLEnum.CullFace);
        gl.Disable(GLEnum.DepthTest);

        gl.ColorMask(true, true, true, true);

        gl.Enable(GLEnum.Blend);
        gl.BlendEquationSeparate(GLEnum.FuncAdd, GLEnum.FuncAdd);
        gl.BlendFuncSeparate(GLEnum.One, GLEnum.OneMinusSrcAlpha, GLEnum.OneMinusDstAlpha, GLEnum.One);

        var widthInPoints = width / pixelsPerPoint;
        var heightInPoints = height / pixelsPerPoint;

        gl.Viewport(0, 0, width, height);
        gl.UseProgram(shaderProgram);

        gl.Uniform2(uScreenSize, [widthInPoints, heightInPoints]);
        gl.Uniform1(uSampler, 0);
        gl.ActiveTexture(GLEnum.Texture0);

        gl.BindVertexArray(vao);
        gl.BindBuffer(GLEnum.ElementArrayBuffer, eao);
    }

    void RenderPrimitives(uint width, uint height, float pixelsPerPoint, ReadOnlyMemory<ClippedPrimitive> primitives)
    {
        PrepareRendering(width, height, pixelsPerPoint);

        foreach (var primitive in primitives.Span)
        {
            switch (primitive.Primitive.Inner)
            {
                case Primitive.Mesh meshPrimitive:
                {
                    Mesh mesh = meshPrimitive.Value;

                    SetClipRect(width, height, pixelsPerPoint, primitive.ClipRect);

                    var texture = textures[mesh.TextureId];
                    gl.BindBuffer(GLEnum.ArrayBuffer, vbo);

                    fixed (Vertex* vertices = mesh.Vertices.AsSpan())
                    {
                        gl.BufferData(GLEnum.ArrayBuffer, (nuint)(mesh.Vertices.Length * sizeof(Vertex)), vertices, BufferUsageARB.StreamDraw);
                    }

                    gl.BindBuffer(GLEnum.ElementArrayBuffer, eao);

                    fixed (uint* indices = mesh.Indices.AsSpan())
                    {
                        gl.BufferData(GLEnum.ElementArrayBuffer, (nuint)(mesh.Indices.Length * sizeof(uint)), indices, BufferUsageARB.StreamDraw);
                    }

                    gl.BindTexture(GLEnum.Texture2D, texture);
                    gl.DrawElements(GLEnum.Triangles, (uint)mesh.Indices.Length, GLEnum.UnsignedInt, null);
                    break;
                }
            }
        }
    }

    public void FreeTexture(TextureId id)
    {
        textures.Remove(id, out var handle);
        gl.DeleteTexture(handle);
    }

    public void SetTexture(TextureId id, ImageDelta delta)
    {
        if (!textures.ContainsKey(id)) textures[id] = gl.GenTexture();

        CheckGlErrors();
        gl.BindTexture(GLEnum.Texture2D, textures[id]);
        CheckGlErrors();

        switch (delta.Image.Inner)
        {
            case ImageData.Color image:
            {
                #pragma warning disable CS9193
                gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GlowCode(delta.Options.Magnification, null));
                gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GlowCode(delta.Options.Minification, delta.Options.MipmapMode));
                gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)GlowCode(delta.Options.WrapMode));
                gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)GlowCode(delta.Options.WrapMode));
                #pragma warning restore CS9193

                CheckGlErrors();
                gl.PixelStore(GLEnum.UnpackAlignment, 1);
                CheckGlErrors();

                fixed (Color32* data = image.Value.Pixels.ToArray())
                {
                    if (delta.Pos is null)
                    {
                        gl.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Rgba8, (uint)image.Value.Size[0], (uint)image.Value.Size[1], 0, GLEnum.Rgba, GLEnum.UnsignedByte,
                            data);
                    }
                    else
                    {
                        gl.TexSubImage2D(GLEnum.Texture2D, 0, (int)delta.Pos.Value[0], (int)delta.Pos.Value[1], (uint)image.Value.Size[0], (uint)image.Value.Size[1], GLEnum.Rgba, GLEnum.UnsignedByte,
                            data);
                    }
                }
                CheckGlErrors();

                if (delta.Options.MipmapMode.HasValue)
                {
                    gl.GenerateMipmap(GLEnum.Texture2D);
                }
                CheckGlErrors();
                break;
            }
        }
    }

    void SetClipRect(uint width, uint height, float pixelsPerPoint, Rect clipRect)
    {
        var clipMinXf = pixelsPerPoint * clipRect.Min.X;
        var clipMinYf = pixelsPerPoint * clipRect.Min.Y;
        var clipMaxXf = pixelsPerPoint * clipRect.Max.X;
        var clipMaxYf = pixelsPerPoint * clipRect.Max.Y;

        var clipMinX = (int)MathF.Round(clipMinXf);
        var clipMinY = (int)MathF.Round(clipMinYf);
        var clipMaxX = (int)MathF.Round(clipMaxXf);
        var clipMaxY = (int)MathF.Round(clipMaxYf);

        clipMinX = Math.Clamp(clipMinX, 0, (int)width);
        clipMinY = Math.Clamp(clipMinY, 0, (int)height);
        clipMaxX = Math.Clamp(clipMaxX, clipMinX, (int)width);
        clipMaxY = Math.Clamp(clipMaxY, clipMinY, (int)height);

        gl.Scissor(clipMinX, (int)height - clipMaxY, (uint)(clipMaxX - clipMinX), (uint)(clipMaxY - clipMinY));
    }

    uint CompileShader(GLEnum shaderType, string text)
    {
        var shader = gl.CreateShader(shaderType);
        gl.ShaderSource(shader, text);
        gl.CompileShader(shader);

        gl.GetShader(shader, ShaderParameterName.CompileStatus, out int status);

        if ((GLEnum)status != GLEnum.True) throw new Exception("Shader failed to compile: " + gl.GetShaderInfoLog(shader));

        return shader;
    }

    uint LinkProgram(ReadOnlySpan<uint> shaders)
    {
        var result = gl.CreateProgram();
        foreach (var shader in shaders)
        {
            gl.AttachShader(result, shader);
        }

        gl.LinkProgram(result);

        gl.GetProgram(result, ProgramPropertyARB.LinkStatus, out int status);
        if ((GLEnum)status != GLEnum.True)
        {
            throw new Exception("Program failed to link: " + gl.GetProgramInfoLog(result));
        }

        return result;
    }

    GLEnum GlowCode(Egui.TextureWrapMode mode)
    {
        if (mode == Egui.TextureWrapMode.ClampToEdge) return GLEnum.ClampToEdge;
        else if (mode == Egui.TextureWrapMode.MirroredRepeat) return GLEnum.MirroredRepeat;
        else return GLEnum.Repeat;
    }

    GLEnum GlowCode(TextureFilter filter, TextureFilter? mipmapMode)
    {
        if (mipmapMode.HasValue)
        {
            if (mipmapMode.Value == TextureFilter.Linear)
            {
                if (filter == TextureFilter.Linear) return GLEnum.LinearMipmapLinear;
                else return GLEnum.NearestMipmapLinear;
            }
            else
            {
                if (filter == TextureFilter.Linear) return GLEnum.LinearMipmapNearest;
                else return GLEnum.NearestMipmapNearest;
            }
        }
        else
        {
            if (filter == TextureFilter.Linear) return GLEnum.Linear;
            else return GLEnum.Nearest;
        }
    }

    void CheckGlErrors()
    {
        var error = gl.GetError();
        if (error != GLEnum.NoError) throw new Exception($"GL error: {error}");
    }

    string FragShader()
    {
        string temp = 
        @"
            #version 140

            #define NEW_SHADER_INTERFACE 1
            #define DITHERING 1

            #ifdef GL_ES
                #if defined(GL_FRAGMENT_PRECISION_HIGH) && GL_FRAGMENT_PRECISION_HIGH == 1
                    precision highp float;
                #else
                    precision mediump float;
                #endif
            #endif

            uniform sampler2D u_sampler;

            #if NEW_SHADER_INTERFACE
                in vec4 v_rgba_in_gamma;
                in vec2 v_tc;
                out vec4 f_color;
                #define gl_FragColor f_color
                #define texture2D texture
            #else
                varying vec4 v_rgba_in_gamma;
                varying vec2 v_tc;
            #endif

            float interleaved_gradient_noise(vec2 n)
            {
                float f = 0.06711056 * n.x + 0.00583715 * n.y;
                return fract(52.9829189 * fract(f));
            }

            vec3 dither_interleaved(vec3 rgb, float levels)
            {
                float noise = interleaved_gradient_noise(gl_FragCoord.xy);
                noise = (noise - 0.5) * 0.95;
                return rgb + noise / (levels - 1.0);
            }

            void main()
            {
                vec4 texture_in_gamma = texture2D(u_sampler, v_tc);
                vec4 frag_color_gamma = v_rgba_in_gamma * texture_in_gamma;
            #if DITHERING
                frag_color_gamma.rgb = dither_interleaved(frag_color_gamma.rgb, 256.);
            #endif
                gl_FragColor = frag_color_gamma;
            }
        ";

        return temp;
    }

    string VertShader()
    {
        string temp = 
        @"
            #version 140

            #define NEW_SHADER_INTERFACE 1

            #if NEW_SHADER_INTERFACE
                #define I in
                #define O out
                #define V(x) x
            #else
                #define I attribute
                #define O varying
                #define V(x) vec3(x)
            #endif

            #ifdef GL_ES
                #if defined(GL_FRAGMENT_PRECISION_HIGH) && GL_FRAGMENT_PRECISION_HIGH == 1
                    precision highp float;
                #else
                    precision mediump float;
                #endif
            #endif

            uniform vec2 u_screen_size;

            I vec2 a_pos;
            I vec4 a_srgba;
            I vec2 a_tc;
            O vec4 v_rgba_in_gamma;
            O vec2 v_tc;

            void main()
            {
                gl_Position = vec4(2.0 * a_pos.x / u_screen_size.x - 1.0, 1.0 - 2.0 * a_pos.y / u_screen_size.y, 0.0, 1.0);
                v_rgba_in_gamma = a_srgba / 255.0;
                v_tc = a_tc;
            }
        ";

        return temp;
    }
}