using Sparebeat.Core;
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
            _sparebeat.Loadded += Browser_Loadded;
        }

        private async void Browser_Loadded(object sender, System.EventArgs e)
        {
            root.Children.Remove(imgLogo);

            var client = new SparebeatClient(AppEnvironment.Songs);
            var map = await client.GetBeatmap("rokuchonen");

            await _sparebeat.Load(map);
        }
    }
}
