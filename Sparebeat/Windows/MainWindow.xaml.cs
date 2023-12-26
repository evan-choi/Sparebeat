using Sparebeat.Common;
using Sparebeat.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sparebeat.Windows;

public partial class MainWindow
{
    private readonly SparebeatClient _client;

    public MainWindow()
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
        if (sender is ListViewItem { DataContext: BeatmapInfo info })
        {
            Start(info);
        }
    }

    private async void Start(BeatmapInfo beatmapInfo)
    {
        var beatmap = await _client.GetBeatmap(beatmapInfo);
        var gameWindow = new GameWindow(beatmapInfo, beatmap);

        gameWindow.FormClosed += delegate { Show(); };
        Hide();

        gameWindow.Show();
    }
}
