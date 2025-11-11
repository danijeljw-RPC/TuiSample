using Terminal.Gui;

namespace TuiSample;

public static class Program
{
    public static void Main()
    {
        Application.Init();
        var shell = new App.MainShell();
        shell.ShowLogin();
        Application.Run();
        Application.Shutdown();
    }
}
