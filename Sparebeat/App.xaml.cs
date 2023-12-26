using System;
using CefSharp;
using CefSharp.WinForms;
using Sparebeat.Handler;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

namespace Sparebeat;

public partial class App
{
    public const string Name = "Sparebeat";

    public App()
    {
        var cefResourcePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "runtimes",
            RuntimeInformation.RuntimeIdentifier,
            "native"
        );

        var libcef = Path.Combine(cefResourcePath, "libcef.dll");
        string message = null;

        try
        {
            using var handle = new CefLibraryHandle(libcef);

            if (handle.IsInvalid)
                message = "CefSharp initialize failed";
        }
        catch (Exception e)
        {
            message = $"CefSharp initialize failed: {e.Message}";
        }

        if (message != null)
        {
            MessageBox.Show(message, "Sparebeat", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
            return;
        }

        InitializeCefSharp(cefResourcePath);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void InitializeCefSharp(string resourceDirPath)
    {
        var settings = new CefSettings()
        {
            ResourcesDirPath = resourceDirPath,
            BrowserSubprocessPath = Path.Combine(resourceDirPath, "CefSharp.BrowserSubprocess.exe"),
            LocalesDirPath = Path.Combine(resourceDirPath, "locales"),
            CachePath = AppEnvironment.Cache,
            CefCommandLineArgs = { { "enable-media-stream", "1" } },
#if RELEASE
            LogSeverity = LogSeverity.Disable
#endif
        };

        settings.RegisterScheme(new CefCustomScheme
        {
            SchemeName = "app",
            SchemeHandlerFactory = new AppSchemeHandlerFactory()
        });

        Cef.Initialize(settings, false, browserProcessHandler: null);
    }
}
