using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TimePunch_WinUI_StackedUI_Demo;
using TimePunch_WinUI_StackedUI_Demo.Core;
using Uno.Resizetizer;

namespace TimePunch_Uno_StackedUI_Demo;
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        Console.WriteLine("[TPUNO] App ctor start");
        this.InitializeComponent();
        Console.WriteLine("[TPUNO] App ctor done");
    }

    private Window? _window;

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Console.WriteLine("[TPUNO] App.OnLaunched start");
        Debug.WriteLine("[TPUNO] App.OnLaunched start");

#if __WASM__
        Console.WriteLine("[TPUNO] App.OnLaunched before CreateBuilder");
        var builder = this.CreateBuilder(args);
        _window = builder.Window;
        Console.WriteLine("[TPUNO] App.OnLaunched after CreateBuilder");
#else
        Console.WriteLine("[TPUNO] App.OnLaunched before base.OnLaunched");
        base.OnLaunched(args);
        Console.WriteLine("[TPUNO] App.OnLaunched after base.OnLaunched");
        Console.WriteLine("[TPUNO] App.OnLaunched before new Window");
        _window = new Window();
        Console.WriteLine("[TPUNO] App.OnLaunched after new Window");
#endif

#if DEBUG && !__WASM__
        _window.EnableHotReload();
#endif
        Console.WriteLine("[TPUNO] App.OnLaunched before DemoKernel.Instance");
        DemoKernel.Instance.AppWindow = _window;
        Console.WriteLine("[TPUNO] App.OnLaunched after DemoKernel.Instance");

        if (_window.Content is not Frame rootFrame)
        {
            Console.WriteLine("[TPUNO] App.OnLaunched create root Frame");
            Debug.WriteLine("[TPUNO] App.OnLaunched create root Frame");
            rootFrame = new Frame();
            _window.Content = rootFrame;
            rootFrame.NavigationFailed += OnNavigationFailed;
        }

        if (rootFrame.Content == null)
        {
            Console.WriteLine("[TPUNO] App.OnLaunched navigate MainWindow");
            Debug.WriteLine("[TPUNO] App.OnLaunched navigate MainWindow");
            rootFrame.Navigate(typeof(MainWindow), args.Arguments);
        }

#if !__WASM__
        _window.SetWindowIcon();
#endif
        Console.WriteLine("[TPUNO] App.OnLaunched activate window");
        Debug.WriteLine("[TPUNO] App.OnLaunched activate window");
        _window.Activate();
        Console.WriteLine("[TPUNO] App.OnLaunched done");
        Debug.WriteLine("[TPUNO] App.OnLaunched done");
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
    }

    /// <summary>
    /// Configures global Uno Platform logging
    /// </summary>
    public static void InitializeLogging()
    {
#if DEBUG
        var factory = LoggerFactory.Create(builder =>
        {
#if __WASM__
            builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__ || __MACCATALYST__
            builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#else
            builder.AddConsole();
#endif
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddFilter("Uno", LogLevel.Warning);
            builder.AddFilter("Windows", LogLevel.Warning);
            builder.AddFilter("Microsoft", LogLevel.Warning);
        });

        global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
        global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
#endif
    }
}
