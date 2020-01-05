using Sparebeat.Core;
using System.Windows;

namespace Sparebeat
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var sparebeatBrowser = new SparebeatBrowser(browser);
            sparebeatBrowser.PageLoaded += Browser_PageLoaded;
        }

        private void Browser_PageLoaded(object sender, System.EventArgs e)
        {
            root.Children.Remove(imgLogo);
        }
    }
}
