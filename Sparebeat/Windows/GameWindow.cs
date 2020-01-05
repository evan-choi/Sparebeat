using CefSharp.WinForms;
using Sparebeat.Common;
using Sparebeat.Core;
using Sparebeat.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sparebeat.Windows
{
    public class GameWindow : Form
    {
        private PictureBox _logo;
        private ChromiumWebBrowser _browser;

        private SparebeatBrowser _sparebeat;

        public GameWindow()
        {
            InitializeComponent();

            _sparebeat = new SparebeatBrowser(_browser);
            _sparebeat.Loadded += Sparebeat_Loadded;
            _sparebeat.BeatmapChanged += Sparebeat_BeatmapChanged;
        }

        private void InitializeComponent()
        {
            Text = Program.Name;
            Icon = Resources.Icon;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(960, 640);
            BackColor = Color.FromArgb(34, 34, 34);

            _logo = new PictureBox();
            _logo.Image = Resources.Logo;
            _logo.SizeMode = PictureBoxSizeMode.StretchImage;
            _logo.Dock = DockStyle.None;
            _logo.Size = new Size(128, 128);
            _logo.Location = new Point((ClientSize.Width - _logo.Size.Width) / 2, 200);

            _browser = new ChromiumWebBrowser("app://main.html");
            _browser.Size = ClientSize;
            _browser.Left = ClientSize.Width;
            _browser.Dock = DockStyle.None;

            Controls.Add(_logo);
            Controls.Add(_browser);
        }

        private async void Sparebeat_Loadded(object sender, EventArgs e)
        {
            Controls.Remove(_logo);
            _browser.Dock = DockStyle.Fill;

            var client = new SparebeatClient(AppEnvironment.Songs);
            var infos = await client.GetBeatmapInfos();
            var map = await client.GetBeatmap(infos[34]);

            await _sparebeat.Load(map);
        }

        private void Sparebeat_BeatmapChanged(object sender, Beatmap beatmap)
        {
            if (beatmap != null)
                Text = $"{Program.Name} - {beatmap.Metadata.Title}";
            else
                Text = Program.Name;
        }
    }
}