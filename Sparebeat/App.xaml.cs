using System;
using System.Collections.Generic;
using CefSharp;
using CefSharp.WinForms;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using Sparebeat.Core;
using Sparebeat.Handler;

namespace Sparebeat;

public partial class App
{
    public const string Name = "Sparebeat";

    public App()
    {
        try
        {
            var cefDif = GetCefDirectory();

            InitializeCefSharp(cefDif);
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Sparebeat", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }

    private static string GetCefDirectory()
    {
        return CefDirectoryCandidates()
            .Where(Directory.Exists)
            .FirstOrDefault(x =>
            {
                var libcef = Path.Combine(x, "libcef.dll");

                if (!File.Exists(libcef))
                    return false;

                using var handle = new CefLibraryHandle(libcef);
                return !handle.IsInvalid;
            }) ?? throw new DllNotFoundException("CEF initialize failed");
    }

    private static IEnumerable<string> CefDirectoryCandidates()
    {
        yield return Path.Combine(
            Directory.GetCurrentDirectory(),
            "runtimes",
            RuntimeInformation.RuntimeIdentifier,
            "native"
        );

        yield return Directory.GetCurrentDirectory();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void InitializeCefSharp(string resourceDirPath)
    {
        var settings = new CefSettings
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

        CefSharpSettings.ShutdownOnExit = true;

        Cef.Initialize(settings, false, browserProcessHandler: null);
    }
}
