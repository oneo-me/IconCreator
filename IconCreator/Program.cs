using Avalonia;
using OpenView.Desktop;

namespace IconCreator;

static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return DesktopApp.Build<App>();
    }
}
