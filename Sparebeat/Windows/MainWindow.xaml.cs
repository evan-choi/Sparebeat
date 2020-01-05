using Sparebeat.Common;
using Sparebeat.Core;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sparebeat.Windows
{
    public partial class TestWindow : Window
    {
        private SparebeatClient _client;

        public TestWindow()
        {
            InitializeComponent();
            
            _client = new SparebeatClient(AppEnvironment.Songs);
            Loaded += TestWindow_Loaded;
        }

        private async void TestWindow_Loaded(object sender, RoutedEventArgs e)
        {
            lvBeatmap.ItemsSource = await _client.GetBeatmapInfos();
        }

        private void BeatmapInfo_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item && item.DataContext is BeatmapInfo info)
            {
                Start(info);
            }
        }

        private async void Start(BeatmapInfo beatmapInfo)
        {
            var beatmap = await _client.GetBeatmap(beatmapInfo);
            var gameWindow = new GameWindow(beatmap);

            gameWindow.FormClosed += (s, e) => Show();
            Hide();

            System.Windows.Forms.Application.SetHighDpiMode(System.Windows.Forms.HighDpiMode.SystemAware);
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(gameWindow);
        }
    }
}
