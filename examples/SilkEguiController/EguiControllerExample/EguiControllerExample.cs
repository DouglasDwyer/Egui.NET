using Egui;
using Egui.Widgets;
using EguiWindow = Egui.Containers.Window;

using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

public static unsafe class EguiControllerExample
{
    static GL gl;
    static IWindow window;
    static IInputContext input;

    // create the controller
    static EguiController eguiController;

    static float testSliderValue = 50f;

    static void Main()
    {
        var options = WindowOptions.Default;
        options.Title = "Egui Controller Example";
        window = Window.Create(options);
        window.Load += Load;
        window.Render += Render;
        window.Run();
        window.Dispose();
    }

    static void Load()
    {
        input = window.CreateInput();
        gl = GL.GetApi(window);

        // create our imgui controller and pass silk refs
        eguiController = new EguiController(gl, window, input);
    }

    static void Render(double deltaTime)
    {
        // render your stuff
        gl.ClearColor(System.Drawing.Color.AliceBlue);
        gl.Clear((uint)GLEnum.ColorBufferBit);

        // here we define our gui
        var contextAction = (Context context) =>
        {
            // custom egui window
            new EguiWindow("this is an egui window").Show(context, (ui) =>
            {
                ui.Label("this is a text label");

                ui.Add(new Slider<float>(ref testSliderValue, 0f, 100f).Text("egui test slider"));

                if (ui.Button("this is a button").Clicked) Console.WriteLine("first button clicked");
                if (ui.Button("this is another button").Clicked) Console.WriteLine("second button clicked");
            });

            // build in egui example window
            new EguiWindow("egui test").Show(context, (ui) => context.SettingsUi(ui));
        };

        // render egui on top
        eguiController.Render(contextAction);
    }
}