using Sparebeat.Common;
using Sparebeat.Core;
using System;
using System.Windows;

namespace Sparebeat
{
    public partial class MainWindow : Window
    {
        private SparebeatBrowser _sparebeat;

        public MainWindow()
        {
            InitializeComponent();

            _sparebeat = new SparebeatBrowser(browser);
            _sparebeat.Loadded += Sparebeat_Loadded;
            _sparebeat.BeatmapChanged += Sparebeat_BeatmapChanged;
        }

        private async void Sparebeat_Loadded(object sender, EventArgs e)
        {
            root.Children.Remove(imgLogo);

            var client = new SparebeatClient(AppEnvironment.Songs);
            var infos = await client.GetBeatmapInfos();
            var map = await client.GetBeatmap(infos[34]);

            await _sparebeat.Load(map);
        }

        private void Sparebeat_BeatmapChanged(object sender, Beatmap beatmap)
        {
            if (beatmap != null)
                Title = $"{App.Name} - {beatmap.Metadata.Title}";
            else
                Title = App.Name;
        }
    }
}
