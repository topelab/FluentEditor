using Avalonia;
using System;

[assembly: System.Resources.NeutralResourcesLanguageAttribute("en")]

namespace FluentEditor;

internal class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>().UsePlatformDetect().AfterSetup(_ =>
        {
#if DEBUG
            Application.Current!.AttachDevTools();
#endif
        });
    }
}
