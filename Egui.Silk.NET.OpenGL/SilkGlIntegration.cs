using Egui.Epaint;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Collections.Immutable;

namespace Egui.Silk.NET;

/// <summary>
/// Handles user input and rendering for <c>Egui</c> atop an OpenGL window.
/// </summary>
public unsafe class SilkGlIntegration : SilkIntegration
{
    /// <summary>
    /// The fragment shader to use when drawing <c>Egui</c> meshes.
    /// </summary>
    private readonly static string FragShader = @"
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

    /// <summary>
    /// The vertex shader to use when drawing <c>Egui</c> meshes.
    /// </summary>
    private readonly static string VertShader = @"
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

    /// <summary>
    /// Whether this object has been freed.
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// The element buffer object for meshes.
    /// </summary>
    private readonly uint _eao;

    /// <summary>
    /// The GL context.
    /// </summary>
    private readonly GL _gl;

    /// <summary>
    /// A handle to the shader.
    /// </summary>
    private readonly uint _shaderProgram;

    /// <summary>
    /// The set of currently-allocated textures.
    /// </summary>
    private readonly Dictionary<TextureId, uint> _textures;

    /// <summary>
    /// A handle to the sampler uniform.
    /// </summary>
    private readonly int _uSampler;

    /// <summary>
    /// A handle to the screen size uniform.
    /// </summary>
    private readonly int _uScreenSize;

    /// <summary>
    /// The vertex array object for meshes.
    /// </summary>
    private readonly uint _vao;

    /// <summary>
    /// The vertex buffer object for meshes.
    /// </summary>
    private readonly uint _vbo;

    /// <inheritdoc cref="SilkIntegration(Context, IWindow)"/>
    public SilkGlIntegration(Context context, IWindow window) : base(context, window)
    {
        _disposed = false;
        _gl = window.CreateOpenGL();
        
        CheckGlErrors();

        var frag = CompileShader(GLEnum.FragmentShader, FragShader);
        var vert = CompileShader(GLEnum.VertexShader, VertShader);
        _shaderProgram = LinkProgram([vert, frag]);
        _gl.DeleteShader(frag);
        _gl.DeleteShader(vert);

        _uScreenSize = _gl.GetUniformLocation(_shaderProgram, "u_screen_size");
        _uSampler = _gl.GetUniformLocation(_shaderProgram, "u_sampler");

        _vbo = _gl.GenBuffer();

        CheckGlErrors();

        var aPosLoc = _gl.GetAttribLocation(_shaderProgram, "a_pos");
        var aTcLoc = _gl.GetAttribLocation(_shaderProgram, "a_tc");
        var aSrgbaLoc = _gl.GetAttribLocation(_shaderProgram, "a_srgba");

        CheckGlErrors();

        _vao = _gl.GenVertexArray();
        CheckGlErrors();
        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(GLEnum.ArrayBuffer, _vbo);
        CheckGlErrors();

        _gl.EnableVertexAttribArray((uint)aPosLoc);
        _gl.VertexAttribPointer((uint)aPosLoc, 2, GLEnum.Float, false, (uint)sizeof(Vertex), 0);
        CheckGlErrors();
        _gl.EnableVertexAttribArray((uint)aTcLoc);
        CheckGlErrors();
        _gl.VertexAttribPointer((uint)aTcLoc, 2, GLEnum.Float, false, (uint)sizeof(Vertex), 2 * 4);
        CheckGlErrors();
        _gl.EnableVertexAttribArray((uint)aSrgbaLoc);
        _gl.VertexAttribPointer((uint)aSrgbaLoc, 4, GLEnum.UnsignedByte, false, (uint)sizeof(Vertex), 4 * 4);

        CheckGlErrors();

        _eao = _gl.GenBuffer();

        _textures = [];
        CheckGlErrors();
    }

    /// <summary>
    /// Disposes the integration object.
    /// </summary>
    ~SilkGlIntegration()
    {
        Dispose();
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;

            _gl.DeleteProgram(_shaderProgram);
            _gl.DeleteVertexArray(_vao);
            _gl.DeleteBuffer(_vbo);

            foreach (var texture in _textures.Values)
            {
                _gl.DeleteTexture(texture);
            }

            base.Dispose();
        }
    }

    /// <inheritdoc/>
    protected override void DrawOutput(in FullOutput output)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _gl.Disable(GLEnum.ScissorTest);
        _gl.Viewport(0, 0, (uint)Window.FramebufferSize.X, (uint)Window.FramebufferSize.Y);
        var clippedPrimitives = EguiContext.Tessellate(output.Shapes.ToImmutableArray(), output.PixelsPerPoint);
        RenderAndUpdateTextures((uint)Window.FramebufferSize.X, (uint)Window.FramebufferSize.Y, output.PixelsPerPoint, clippedPrimitives.AsSpan(), output.TexturesDelta);
        _gl.Disable(GLEnum.ScissorTest);
    }

    /// <summary>
    /// Allocates new textures, draws all meshes, and then deallocates old textures.
    /// </summary>
    /// <param name="width">The screen width.</param>
    /// <param name="height">The screen height.</param>
    /// <param name="pixelsPerPoint">The number of pixels per point.</param>
    /// <param name="clippedPrimitives">The set of primitives to render.</param>
    /// <param name="texturesDelta">Texture changes for this frame.</param>
    private void RenderAndUpdateTextures(uint width, uint height, float pixelsPerPoint, ReadOnlySpan<ClippedPrimitive> clippedPrimitives, TexturesDelta texturesDelta)
    {
        CheckGlErrors();
        foreach (var (id, delta) in texturesDelta.Set) SetTexture(id, delta);
        CheckGlErrors();
        RenderPrimitives(width, height, pixelsPerPoint, clippedPrimitives);
        CheckGlErrors();
        foreach (var id in texturesDelta.Free) FreeTexture(id);
        CheckGlErrors();
    }

    /// <summary>
    /// Sets up the GL state for GUI rendering.
    /// </summary>
    /// <param name="width">The screen width.</param>
    /// <param name="height">The screen height.</param>
    /// <param name="pixelsPerPoint">The scale factor.</param>
    private void PrepareRendering(uint width, uint height, float pixelsPerPoint)
    {
        _gl.Enable(GLEnum.ScissorTest);
        _gl.Disable(GLEnum.CullFace);
        _gl.Disable(GLEnum.DepthTest);

        _gl.ColorMask(true, true, true, true);

        _gl.Enable(GLEnum.Blend);
        _gl.BlendEquationSeparate(GLEnum.FuncAdd, GLEnum.FuncAdd);
        _gl.BlendFuncSeparate(GLEnum.One, GLEnum.OneMinusSrcAlpha, GLEnum.OneMinusDstAlpha, GLEnum.One);

        var widthInPoints = width / pixelsPerPoint;
        var heightInPoints = height / pixelsPerPoint;

        _gl.Viewport(0, 0, width, height);
        _gl.UseProgram(_shaderProgram);

        _gl.Uniform2(_uScreenSize, [widthInPoints, heightInPoints]);
        _gl.Uniform1(_uSampler, 0);
        _gl.ActiveTexture(GLEnum.Texture0);

        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(GLEnum.ElementArrayBuffer, _eao);
    }

    private void RenderPrimitives(uint width, uint height, float pixelsPerPoint, ReadOnlySpan<ClippedPrimitive> primitives)
    {
        PrepareRendering(width, height, pixelsPerPoint);

        foreach (var primitive in primitives)
        {
            switch (primitive.Primitive.Inner)
            {
                case Primitive.Mesh meshPrimitive:
                {
                    Mesh mesh = meshPrimitive.Value;

                    SetClipRect(width, height, pixelsPerPoint, primitive.ClipRect);

                    var texture = _textures[mesh.TextureId];
                    _gl.BindBuffer(GLEnum.ArrayBuffer, _vbo);

                    fixed (Vertex* vertices = mesh.Vertices.AsSpan())
                    {
                        _gl.BufferData(GLEnum.ArrayBuffer, (nuint)(mesh.Vertices.Length * sizeof(Vertex)), vertices, BufferUsageARB.StreamDraw);
                    }

                    _gl.BindBuffer(GLEnum.ElementArrayBuffer, _eao);

                    fixed (uint* indices = mesh.Indices.AsSpan())
                    {
                        _gl.BufferData(GLEnum.ElementArrayBuffer, (nuint)(mesh.Indices.Length * sizeof(uint)), indices, BufferUsageARB.StreamDraw);
                    }

                    _gl.BindTexture(GLEnum.Texture2D, texture);
                    _gl.DrawElements(GLEnum.Triangles, (uint)mesh.Indices.Length, GLEnum.UnsignedInt, null);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Deallocates a texture.
    /// </summary>
    /// <param name="id">The texture ID to free.</param>
    private void FreeTexture(TextureId id)
    {
        _textures.Remove(id, out var handle);
        _gl.DeleteTexture(handle);
    }

    /// <summary>
    /// Updates the contexts of a texture.
    /// </summary>
    /// <param name="id">The ID of the texture.</param>
    /// <param name="delta">The change in the texture contents.</param>
    private void SetTexture(TextureId id, ImageDelta delta)
    {
        if (!_textures.ContainsKey(id)) _textures[id] = _gl.GenTexture();

        CheckGlErrors();
        _gl.BindTexture(GLEnum.Texture2D, _textures[id]);
        CheckGlErrors();

        switch (delta.Image.Inner)
        {
            case ImageData.Color image:
            {
                #pragma warning disable CS9193
                _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GlCode(delta.Options.Magnification, null));
                _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GlCode(delta.Options.Minification, delta.Options.MipmapMode));
                _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)GlCode(delta.Options.WrapMode));
                _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)GlCode(delta.Options.WrapMode));
                #pragma warning restore CS9193

                CheckGlErrors();
                _gl.PixelStore(GLEnum.UnpackAlignment, 1);
                CheckGlErrors();

                fixed (Color32* data = image.Value.Pixels.ToArray())
                {
                    if (delta.Pos is null)
                    {
                        _gl.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Rgba8, (uint)image.Value.Size[0], (uint)image.Value.Size[1], 0, GLEnum.Rgba, GLEnum.UnsignedByte,
                            data);
                    }
                    else
                    {
                        _gl.TexSubImage2D(GLEnum.Texture2D, 0, (int)delta.Pos.Value[0], (int)delta.Pos.Value[1], (uint)image.Value.Size[0], (uint)image.Value.Size[1], GLEnum.Rgba, GLEnum.UnsignedByte,
                            data);
                    }
                }
                CheckGlErrors();

                if (delta.Options.MipmapMode.HasValue)
                {
                    _gl.GenerateMipmap(GLEnum.Texture2D);
                }
                CheckGlErrors();
                break;
            }
        }
    }

    /// <summary>
    /// Sets up the clipping rectangle for a certain mesh.
    /// </summary>
    /// <param name="width">The screen width, in pixels.</param>
    /// <param name="height">The screen height, in pixels.</param>
    /// <param name="pixelsPerPoint">The scale factor.</param>
    /// <param name="clipRect">The subregion of the screen where drawing may occur, in points.</param>
    private void SetClipRect(uint width, uint height, float pixelsPerPoint, Rect clipRect)
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

        _gl.Scissor(clipMinX, (int)height - clipMaxY, (uint)(clipMaxX - clipMinX), (uint)(clipMaxY - clipMinY));
    }

    /// <summary>
    /// Compiles a single shader.
    /// </summary>
    /// <param name="shaderType">The kind of shader that this is.</param>
    /// <param name="text">The GLSL source code to compile.</param>
    /// <returns>A handle to the shader.</returns>
    /// <exception cref="InvalidOperationException">
    /// If a compilation error occurred.
    /// </exception>
    private uint CompileShader(GLEnum shaderType, string text)
    {
        var shader = _gl.CreateShader(shaderType);
        _gl.ShaderSource(shader, text);
        _gl.CompileShader(shader);

        _gl.GetShader(shader, ShaderParameterName.CompileStatus, out int status);

        if ((GLEnum)status != GLEnum.True) throw new InvalidOperationException("Shader failed to compile: " + _gl.GetShaderInfoLog(shader));

        return shader;
    }

    /// <summary>
    /// Combines multiple programs into a shader pipeline.
    /// </summary>
    /// <param name="shaders">The shaders to link.</param>
    /// <returns>
    /// The resultant shader program.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// If the program could not be linked.
    /// </exception>
    private uint LinkProgram(ReadOnlySpan<uint> shaders)
    {
        var result = _gl.CreateProgram();
        foreach (var shader in shaders)
        {
            _gl.AttachShader(result, shader);
        }

        _gl.LinkProgram(result);

        _gl.GetProgram(result, ProgramPropertyARB.LinkStatus, out int status);
        if ((GLEnum)status != GLEnum.True)
        {
            throw new InvalidOperationException("Program failed to link: " + _gl.GetProgramInfoLog(result));
        }

        return result;
    }

    /// <summary>
    /// Converts a <c>Egui</c> texture wrap mode to a GL code.
    /// </summary>
    /// <param name="mode">The wrap mode.</param>
    /// <returns>The associated enum variant.</returns>
    private static GLEnum GlCode(TextureWrapMode mode)
    {
        if (mode == TextureWrapMode.ClampToEdge) return GLEnum.ClampToEdge;
        else if (mode == TextureWrapMode.MirroredRepeat) return GLEnum.MirroredRepeat;
        else return GLEnum.Repeat;
    }

    /// <summary>
    /// Converts <c>Egui</c> texture filtering modes to a GL code.
    /// </summary>
    /// <param name="filter">The texture filter.</param>
    /// <param name="mipmapMode">The mipmap mode to apply.</param>
    /// <returns>The associated enum variant.</returns>
    private static GLEnum GlCode(TextureFilter filter, TextureFilter? mipmapMode)
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

    /// <summary>
    /// Throws an exception if any GL errors occurred.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// If something went wrong with OpenGL.
    /// </exception>
    private void CheckGlErrors()
    {
        var error = _gl.GetError();
        if (error != GLEnum.NoError) throw new InvalidOperationException($"GL error: {error}");
    }
}