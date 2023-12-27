using CefSharp;
using Sparebeat.Common;
using Sparebeat.Utilities;
using System;
using System.Threading.Tasks;

namespace Sparebeat.Core;

internal class SparebeatBrowser
{
    public event EventHandler Loadded;

    private readonly IWebBrowser _browser;
    private readonly ContextUtility.ContextSnapshot _snapshot;

    public bool IsLoaded => !_browser.IsLoading;

    public SparebeatBrowser(IWebBrowser webBrowser)
    {
        _snapshot = this.Snapshot();

        _browser = webBrowser;
        _browser.LoadingStateChanged += LoadingStateChanged;

        _browser.Load("app://main.html");
    }

    private void LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
    {
        if (e.Browser.IsLoading)
            return;

#if DEBUG
        _browser.ShowDevTools();
#endif

        _browser.LoadingStateChanged -= LoadingStateChanged;
        OnPageLoadded();
    }

    private void OnPageLoadded()
    {
        _snapshot.InvokeEvent(Loadded, this, EventArgs.Empty);
    }

    public async Task<bool> Load(Beatmap beatmap)
    {
        if (beatmap == null)
            throw new ArgumentNullException();

        string mapJson = beatmap.MapJson;
        string musicBin = Convert.ToBase64String(beatmap.Music);

        string script = $"Sparebeat.load({mapJson}, '{musicBin}')";
        var response = await _browser.EvaluateScriptAsync(script);

        return response.Success;
    }
}
