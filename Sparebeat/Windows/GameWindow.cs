using CefSharp;
using Sparebeat.Common;
using Sparebeat.Core;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using CefSharp.Internals;
using Sparebeat.Handler;

namespace Sparebeat.Windows;

internal partial class GameWindow
{
    private Beatmap _beatmap;
    private SparebeatBrowser _sparebeat;

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
        _browser.Hide();
        base.OnFormClosing(e);
    }

    protected override void Dispose(bool disposing)
    {
        _browser.Dispose();
        base.Dispose(disposing);
    }
}