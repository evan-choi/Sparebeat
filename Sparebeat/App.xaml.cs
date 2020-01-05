using CefSharp;
using CefSharp.Wpf;
using Sparebeat.Handler;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

namespace Sparebeat
{
    public partial class App : Application
    {
        public App()
        {
            string cefResourcePath = Path.Combine(Directory.GetCurrentDirectory(), @"resources\cefsharp");
            string libcef = Path.Combine(cefResourcePath, "libcef.dll");

            if (new CefLibraryHandle(libcef).IsInvalid)
            {
                MessageBox.Show("CefSharp initialize failed", "Sparebeat", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            InitializeCefSharp(cefResourcePath);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void InitializeCefSharp(string resourceDirPath)
        {
            var settings = new CefSettings()
            {
                ResourcesDirPath = resourceDirPath,
                BrowserSubprocessPath = Path.Combine(resourceDirPath, "CefSharp.BrowserSubprocess.exe"),
                LocalesDirPath = Path.Combine(resourceDirPath, "locales"),
                CachePath = @"Cache",
                CefCommandLineArgs = { { "enable-media-stream", "1" } }
            };

            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = "app",
                SchemeHandlerFactory = new AppSchemeHandlerFactory()
            });

            Cef.Initialize(settings, false, browserProcessHandler: null);
        }
    }
}
