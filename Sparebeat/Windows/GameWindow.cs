using Sparebeat.Common;
using Sparebeat.Core;
using System;
using System.Windows.Forms;

namespace Sparebeat.Windows
{
    partial class GameWindow
    {
        private Beatmap _beatmap;
        private SparebeatBrowser _sparebeat;

        public GameWindow(Beatmap beatmap)
        {
            InitializeComponent();

            Text = $"{App.Name} - {beatmap.Metadata.Title}";
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
    }
}