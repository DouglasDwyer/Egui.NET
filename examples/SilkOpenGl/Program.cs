#pragma warning disable

using System.Diagnostics;
using System.Collections.Immutable;

using Egui;
using Egui.Containers;
using Egui.Epaint;
using Egui.EguiExtras;
using Egui.EguiExtras.SyntaxHighlighting;
using Egui.Silk.NET;
using Egui.Viewport;
using Egui.Widgets;
using Button = Egui.Widgets.Button;
using Image = Egui.Widgets.Image;

using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Window = Egui.Containers.Window;
using System.Numerics;
using System.Drawing;

namespace MySilkProgram;

public class Program
{
    private static Context _ctx;

    private static GL _gl;

    private static SilkGlIntegration _integration;

    private static IWindow _window;

    private static WidgetGallery _widgetGallery = new WidgetGallery();

    private static CodeExample _codeExample = new CodeExample();

    private static TableDemo _tableDemo = new TableDemo();

    private static CodeEditorDemo _codeEditorDemo = new CodeEditorDemo();

    public static void Main(string[] args)
    {
        _ctx = new Context();

        WindowOptions options = WindowOptions.Default;
        options.API = GraphicsAPI.Default;
        options.Size = new Vector2D<int>(800, 600);
        options.Title = "My first Silk.NET program!";
        options.API = GraphicsAPI.Default;

        _window = Silk.NET.Windowing.Window.Create(options);
        _window.Load += OnLoad;
        _window.Render += OnRender;
        _window.Run();
    }

    private static string name = "";

    private static int _age;

    private static void OnLoad()
    {
        _gl = _window.CreateOpenGL();
        _integration = new SilkGlIntegration(_ctx, _window);
    }

    private static void OnRender(double deltaTime)
    {
        // Clear the screen
        _gl.ClearColor(System.Drawing.Color.AliceBlue);
        _gl.Clear((uint)GLEnum.ColorBufferBit);

        // Draw UI
        _integration.Run(rootUi =>
        {
            var ctx = rootUi.Ctx;

            new Window("README Example")
                .FixedSize((160, 160))
                .Show(ctx, ui =>
            {
                ui.Heading("My egui Application");
                ui.Horizontal(ui =>
                {
                    ui.Label("Your name:");
                    ui.TextEditSingleline(ref name);
                });
                ui.Add(new Slider<int>(ref _age, 0, 120).Text("age"));
                if (ui.Button("Increment").Clicked)
                {
                    _age += 1;
                }
                ui.Label($"Hello '{name}', age {_age}");
                ui.Image(EguiHelpers.IncludeImageResource("csharp.png"));
            });

            new Window("🗄 Widget Gallery")
                .Resizable((true, false))
                .DefaultWidth(280)
                .Show(ctx, ui =>
            {
                _widgetGallery.Show(ui);
            });

            new Window("Settings")
                .Show(ctx, ui => ctx.SettingsUi(ui));

            new Window("🖮 Code Example")
                .MinWidth(375)
                .DefaultSize((390, 500))
                .Scroll((false, false))
                .Resizable((true, false))
                .Show(ctx, ui =>
            {
                _codeExample.Show(ui);
            });

            new Window("☰ Table")
                .DefaultWidth(400)
                .Show(ctx, ui =>
            {
                _tableDemo.Show(ui);
            });

            new Window("🖮 Code Editor")
                .DefaultHeight(500)
                .Show(ctx, ui =>
            {
                _codeEditorDemo.Show(ui);
            });
        });
    }

    private class WidgetGallery
    {
        private bool _enabled = true;
        private bool _visible = true;
        private bool _boolean = false;
        private float _opacity = 1.0f;
        private Enum _radio = Enum.First;
        private float _scalar = 42.0f;
        private string _string = "";
        private Color32 _color = Color32.LightBlue.LinearMultiply(0.5f);
        private bool _animateProgressBar = false;
        private DateOnly _date = DateOnly.FromDateTime(DateTime.Today);

        public void Show(Ui ui)
        {
            UiBuilder uiBuilder = new UiBuilder();
            if (!_enabled)
            {
                uiBuilder = uiBuilder.WithDisabled();
            }

            if (!_visible)
            {
                uiBuilder = uiBuilder.WithInvisible();
            }

            ui.ScopeBuilder(uiBuilder, ui =>
            {
                ui.MultiplyOpacity(_opacity);
                new Grid("my_grid")
                    .NumColumns(2)
                    .Spacing((40, 4))
                    .Striped(true)
                    .Show(ui, GalleryGridContents);
            });

            ui.Separator();

            ui.Horizontal(ui =>
            {
                ui.Checkbox(ref _visible, "Visible")
                    .OnHoverText("Uncheck to hide all the widgets.");

                if (_visible)
                {
                    ui.Checkbox(ref _enabled, "Interactive")
                        .OnHoverText("Uncheck to inspect how the widgets look when disabled.");
                    ui.Add(new DragValue<float>(ref _opacity)
                        .Speed(0.01f)
                        .Range(0.0f, 1.0f))// | ui.Label("Opacity")
                        .OnHoverText("Reduce this value to make widgets semi-transparent");
                }
            });

            ui.Separator();

            ui.VerticalCentered(ui =>
            {
                ui.Hyperlink("https://docs.rs/egui/")
                    .OnHoverText("The full egui documentation.\nYou can also click the different widgets names in the left column.");
                ui.HyperlinkTo(new RichText("Source code of the widget gallery").Small(), "https://github.com/emilk/egui/blob/master/crates/egui_demo_lib/src/demo/widget_gallery.rs");
            });
        }

        private void GalleryGridContents(Ui ui)
        {
            ui.Add(DocLinkLabel("Label", "label"));
            ui.Label("Welcome to the widget gallery!");
            ui.EndRow();

            ui.Add(DocLinkLabel("Hyperlink", "Hyperlink"));
            ui.HyperlinkTo(" egui on GitHub", "https://github.com/emilk/egui");
            ui.EndRow();

            ui.Add(DocLinkLabel("TextEdit", "TextEdit"));
            ui.Add(TextEdit.Singleline(ref _string).HintText("Write something here"));
            ui.EndRow();

            ui.Add(DocLinkLabel("Button", "button"));
            _boolean ^= ui.Button("Click me!").Clicked;
            ui.EndRow();

            ui.Add(DocLinkLabel("Link", "link"));
            _boolean ^= ui.Link("Click me!").Clicked;
            ui.EndRow();

            ui.Add(DocLinkLabel("Checkbox", "checkbox"));
            ui.Checkbox(ref _boolean, "Checkbox");
            ui.EndRow();

            ui.Add(DocLinkLabel("RadioButton", "radio"));
            ui.Horizontal(ui =>
            {
                ui.RadioValue(ref _radio, Enum.First, "First");
                ui.RadioValue(ref _radio, Enum.Second, "Second");
                ui.RadioValue(ref _radio, Enum.Third, "Third");
            });
            ui.EndRow();

            ui.Add(DocLinkLabel("SelectableLabel", "SelectableLabel"));
            ui.Horizontal(ui =>
            {
                ui.SelectableValue(ref _radio, Enum.First, "First");
                ui.SelectableValue(ref _radio, Enum.Second, "Second");
                ui.SelectableValue(ref _radio, Enum.Third, "Third");
            });
            ui.EndRow();

            ui.Add(DocLinkLabel("ComboBox", "ComboBox"));
            ComboBox.FromLabel("Take your pick")
                .SelectedText($"{_radio}")
                .ShowUi(ui, ui =>
                {
                    ui.SelectableValue(ref _radio, Enum.First, "First");
                    ui.SelectableValue(ref _radio, Enum.Second, "Second");
                    ui.SelectableValue(ref _radio, Enum.Third, "Third");
                });
            ui.EndRow();

            ui.Add(DocLinkLabel("Slider", "Slider"));
            ui.Add(new Slider<float>(ref _scalar, 0.0f, 360.0f).Suffix("°"));
            ui.EndRow();

            ui.Add(DocLinkLabel("DragValue", "DragValue"));
            ui.Add(new DragValue<float>(ref _scalar).Speed(1));
            ui.EndRow();

            ui.Add(DocLinkLabel("ProgressBar", "ProgressBar"));
            var progress = _scalar / 360.0f;
            var progressBar = new ProgressBar(progress)
                .ShowPercentage()
                .Animate(_animateProgressBar);
            _animateProgressBar = ui.Add(progressBar)
                .OnHoverText("The progress bar can be animated!")
                .Hovered;
            ui.EndRow();

            ui.Add(DocLinkLabel("Color picker", "color_edit"));
            ui.ColorEditButtonSrgba(ref _color);
            ui.EndRow();

            ui.Add(DocLinkLabel("Image", "Image"));
            var eguiIcon = EguiHelpers.IncludeImageResource("icon.png");
            ui.Add(new Image(eguiIcon));
            ui.EndRow();

            ui.Add(DocLinkLabel("Button with image", "Button::image_and_text"));
            if (ui.Add(Button.ImageAndText(eguiIcon, "Click me!")).Clicked)
            {
                _boolean = !_boolean;
            }
            ui.EndRow();

            ui.Add(DocLinkLabel("DatePicker", "DatePickerButton"));
            ui.Add(new DatePicker(ref _date));
            ui.EndRow();

            ui.Add(DocLinkLabel("Separator", "separator"));
            ui.Separator();
            ui.EndRow();

            ui.Add(DocLinkLabel("CollapsingHeader", "collapsing"));
            ui.Collapsing("Click to see what is hidden!", ui =>
            {
                ui.HorizontalWrapped(ui =>
                {
                    // ui.spacing_mut().item_spacing.x = 0.0;
                    ui.Label("It's a ");
                    ui.Add(DocLinkLabel("Spinner", "spinner"));
                    ui.AddSpace(4.0f);
                    ui.Add(new Spinner());
                });
            });
            ui.EndRow();

            ui.Hyperlink("Custom widget");
            ui.Add(new Toggle(ref _boolean))
                .OnHoverText("It's easy to create your own widgets!\nThis toggle switch is just 15 lines of code.");
            ui.EndRow();
        }

        private static DocLinkLabelWidget DocLinkLabel(string title, string searchTerm)
        {
            return new DocLinkLabelWidget
            {
                Title = title,
                SearchTerm = searchTerm
            };
        }

        private struct DocLinkLabelWidget : IWidget
        {
            public required string Title;
            public required string SearchTerm;

            Response IWidget.Ui(Ui ui)
            {
                var searchTerm = SearchTerm;
                return ui.HyperlinkTo(Title, $"https://docs.rs/egui?search={searchTerm}")
                .OnHoverUi(ui =>
                {
                    ui.HorizontalWrapped(ui =>
                    {
                        ui.Label("Search egui docs for");
                        ui.Code(searchTerm);
                    });
                });
            }
        }
    }

    /// <summary>
    /// iOS-style toggle switch.
    /// </summary>
    private ref struct Toggle : IWidget
    {
        /// <summary>
        /// The value to update.
        /// </summary>
        private ref bool _on;

        /// <summary>
        /// Creates a toggle.
        /// </summary>
        /// <param name="on">Whether the toggle should be on.</param>
        public Toggle(ref bool on)
        {
            _on = ref on;
        }

        Response IWidget.Ui(Ui ui)
        {
            // Widget code can be broken up in four steps:
            //  1. Decide a size for the widget
            //  2. Allocate space for it
            //  3. Handle interactions with the widget (if any)
            //  4. Paint the widget

            // 1. Deciding widget size:
            // You can query the `ui` how much space is available,
            // but in this example we have a fixed size widget based on the height of a standard button:
            //var desiredSize = ui.Spacing.InteractSize.Y * new Vec2(2.0f, 1.0f);
            var desiredSize = ui.Spacing.InteractSize.Y * new EVec2(2, 1);

            // 2. Allocating space:
            // This is where we get a region of the screen assigned.
            // We also tell the Ui to sense clicks in the allocated region.
            var (rect, response) = ui.AllocateExactSize(desiredSize, Sense.Click);

            // 3. Interact: Time to check for clicks!
            if (response.Clicked)
            {
                _on = !_on;
                response.MarkChanged();
            }

            // Attach some meta-data to the response which can be used by screen readers:
            var isEnabled = ui.IsEnabled;
            var isOn = _on;
            response.WidgetInfo(() => WidgetInfo.WithSelected(WidgetType.Checkbox, isEnabled, isOn, ""));

            // 4. Paint!
            // Make sure we need to paint:
            if (ui.IsRectVisible(rect))
            {
                // Let's ask for a simple animation from egui.
                // egui keeps track of changes in the boolean associated with the id and
                // returns an animated value in the 0-1 range for how much "on" we are.
                var howOn = ui.Ctx.AnimateBoolResponsive(response.Id, _on);
                // We will follow the current style by asking
                // "how should something that is being interacted with be painted?".
                // This will, for instance, give us different colors when the widget is hovered or clicked.
                var visuals = ui.Style.InteractSelectable(response, _on);
                // All coordinates are in absolute screen coordinates so we use `rect` to place the elements.
                rect = rect.Expand(visuals.Expansion);
                var radius = 0.5f * rect.Height;
                ui.Painter.Rect(
                    rect,
                    //radius,
                    CornerRadius.Same((byte)MathF.Round(radius)),
                    visuals.BgFill,
                    visuals.BgStroke,
                    StrokeKind.Inside
                );
                // Paint the circle, animating it from left to right with `how_on`:
                //var circleX = EguiHelpers.Lerp(rect.Left + radius, rect.Right - radius, howOn);
                var circleX = (1.0f - howOn) * (rect.Left + radius) + howOn * (rect.Right - radius);
                var center = (circleX, rect.Center.Y);
                ui.Painter
                    .Circle(center, 0.75f * radius, visuals.BgFill, visuals.FgStroke);
            }

            // All done! Return the interaction response so the user can check what happened
            // (hovered, clicked, ...) and maybe show a tooltip:
            return response;
        }
    }

    private enum Enum
    {
        First,
        Second,
        Third,
    }

    private class CodeExample
    {
        private const string Snippet = """
            public class CodeExample
            {
                private string name;
                private int age;

                public void Show(Ui ui)
                {
                    ui.Heading("Example");
                    ui.Horizontal(ui =>
                    {
                        ui.Label("Name");
                        ui.TextEditSingleline(ref name);
                    });
                    ui.Add(new DragValue<int>(ref age).Range(0, 120).Suffix(" years"));
                    if (ui.Button("Increment").Clicked)
                    {
                        age += 1;
                    }
                    ui.Label($"{name} is {age}");
                }
            }
            """;

        public void Show(Ui ui)
        {
            var theme = CodeTheme.FromMemory(ui.Ctx, ui.Style);
            SyntaxHighlightingHelpers.CodeViewUi(ui, theme, Snippet, "cs");

            ui.Separator();

            ui.Collapsing("Theme", ui =>
            {
                theme.Ui(ui);
                theme.StoreInMemory(ui.Ctx);
            });
        }
    }

    private class TableDemo
    {
        private const int NumRows = 20;

        private bool _striped = true;
        private bool _resizableColumns = true;
        private bool _clickable = true;
        private HashSet<int> _selection = new HashSet<int>();

        public void Show(Ui ui)
        {
            ui.Horizontal(ui =>
            {
                ui.Checkbox(ref _striped, "Striped");
                ui.Checkbox(ref _resizableColumns, "Resizable columns");
                ui.Checkbox(ref _clickable, "Clickable rows");
            });

            ui.Separator();

            var table = new TableBuilder()
                .Striped(_striped)
                .Resizable(_resizableColumns)
                .Column(Column.Auto())
                .Column(Column.Remainder().AtLeast(80.0f))
                .Column(Column.Remainder().AtLeast(80.0f))
                .MinScrolledHeight(0.0f);

            table.Show(ui, 20.0f, header =>
            {
                header.Col(ui => ui.Label("Row"));
                header.Col(ui => ui.Label("Description"));
                header.Col(ui => ui.Label("Progress"));
            }, body =>
            {
                for (int i = 0; i < NumRows; i++)
                {
                    var index = i;
                    body.Row(18.0f, row =>
                    {
                        row.SetSelected(_selection.Contains(index));

                        row.Col(ui => ui.Label($"{index}"));
                        row.Col(ui =>
                        {
                            if (_clickable)
                            {
                                if (ui.Button($"This is row {index}").Clicked && !_selection.Add(index))
                                {
                                    _selection.Remove(index);
                                }
                            }
                            else
                            {
                                ui.Label($"This is row {index}");
                            }
                        });
                        row.Col(ui => ui.Add(new ProgressBar((index % 10) / 10.0f)));
                    });
                }
            });
        }
    }

    private class CodeEditorDemo
    {
        private string _language = "cs";
        private string _code = "// A very simple example\npublic class Program\n{\n    public static void Main()\n    {\n        System.Console.WriteLine(\"Hello world!\");\n    }\n}\n";

        public void Show(Ui ui)
        {
            ui.Label("An example of syntax highlighting in a TextEdit.");

            ui.Horizontal(ui =>
            {
                ui.Label("Language:");
                ui.TextEditSingleline(ref _language);
            });

            var theme = CodeTheme.FromMemory(ui.Ctx, ui.Style);
            ui.Collapsing("Theme", ui =>
            {
                theme.Ui(ui);
                theme.StoreInMemory(ui.Ctx);
            });

            ScrollArea.Vertical.Show(ui, ui =>
            {
                var editor = TextEdit.Multiline(ref _code)
                    .CodeEditor()
                    .DesiredRows(10)
                    .DesiredWidth(float.PositiveInfinity)
                    .Layouter((layoutUi, text, wrapWidth) =>
                    {
                        var job = SyntaxHighlightingHelpers.Highlight(layoutUi.Ctx, layoutUi.Style, theme, text, _language);
                        job.Wrap.MaxWidth = wrapWidth;
                        return job;
                    });
                ui.Add(editor);
            });
        }
    }
}