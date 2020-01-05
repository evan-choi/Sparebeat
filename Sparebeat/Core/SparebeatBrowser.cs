using CefSharp;
using Sparebeat.Utilities;
using System;

namespace Sparebeat.Core
{
    class SparebeatBrowser
    {
        public event EventHandler PageLoaded;

        private IWebBrowser _browser;
        private ContextUtility.ContextSnapshot _snapshot;

        public SparebeatBrowser(IWebBrowser webBrowser)
        {
            _snapshot = this.Snapshot();

            _browser = webBrowser;
            _browser.LoadingStateChanged += LoadingStateChanged;
        }

        private void LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.Browser.IsLoading)
                return;

            _browser.LoadingStateChanged -= LoadingStateChanged;
            OnPageLoadded();
        }

        private void OnPageLoadded()
        {
            _snapshot.InvokeEvent(PageLoaded, this, EventArgs.Empty);
        }
    }
}
