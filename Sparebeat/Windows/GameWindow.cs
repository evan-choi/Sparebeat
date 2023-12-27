using Sparebeat.Common;
using Sparebeat.Core;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Sparebeat.Windows;

internal partial class GameWindow
{
    private readonly Beatmap _beatmap;
    private readonly SparebeatBrowser _sparebeat;

    public GameWindow(BeatmapInfo info, Beatmap beatmap)
    {
        InitializeComponent();

        Text = $"{App.Name} - {info.Title}";
        _beatmap = beatmap;

        _sparebeat = new SparebeatBrowser(_browser);
        _sparebeat.Loadded += Sparebeat_Loadded;
    }

    private async void Sparebeat_Loadded(object sender, EventArgs e)
    {
        Controls.Remove(_logo);
        _browser.Dock = DockStyle.Fill;

        await _sparebeat.Load(_beatmap);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _browser.BrowserCore.CloseBrowser(false);
    }

    protected override void Dispose(bool disposing)
    {
        _browser.Dispose();
        base.Dispose(disposing);
    }
}
