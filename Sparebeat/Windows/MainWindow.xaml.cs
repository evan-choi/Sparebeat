using System;
using System.Collections.Frozen;
using System.Threading.Tasks;
using Sparebeat.Common;
using Sparebeat.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sparebeat.Windows;

public partial class MainWindow
{
    private readonly SparebeatClient _client;
    private readonly SparebeatLocalStorage _localStorage;

    public MainWindow()
    {
        InitializeComponent();

        _client = new SparebeatClient(AppEnvironment.Songs);
        _localStorage = new SparebeatLocalStorage();

        Loaded += TestWindow_Loaded;
    }

    private async void TestWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await Task.Delay(10);
        BeatmapInfo[] beatmapInfos = await _client.GetBeatmapInfos();
        FrozenDictionary<string, RecordSet> records = await _localStorage.GetRecordsAsync();

        foreach (var beatmapInfo in beatmapInfos)
        {
            if (records.TryGetValue(beatmapInfo.Id, out var recordSet))
                beatmapInfo.RecordSet = recordSet;
        }

        lvBeatmap.ItemsSource = beatmapInfos;
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

        gameWindow.FormClosed += async delegate
        {
            Show();

            try
            {
                var recordSet = await _localStorage.FindRecordAsync(beatmapInfo.Id);

                if (recordSet != null)
                    beatmapInfo.RecordSet = recordSet;
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"Could not find '{beatmapInfo.Title}' record: {e.Message}",
                    "Sparebeat",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        };

        Hide();

        gameWindow.Show();
    }
}
