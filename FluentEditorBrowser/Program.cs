using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;

[assembly: System.Resources.NeutralResourcesLanguageAttribute("en")]
[assembly: SupportedOSPlatform("browser")]

namespace FluentEditor;

internal class Program
{
    private static Task Main(string[] args)
    {
        return BuildAvaloniaApp()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>();
}
