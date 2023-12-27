using CefSharp.WinForms;
using Sparebeat.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace Sparebeat.Windows
{
    partial class GameWindow : Form
    {
        private PictureBox _logo;
        private ChromiumWebBrowser _browser;

        private void InitializeComponent()
        {
            Text = App.Name;
            Icon = Resources.Icon;
            MaximizeBox = false;
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

            _browser = new ChromiumWebBrowser();
            _browser.Size = ClientSize;
            _browser.Left = ClientSize.Width;
            _browser.Dock = DockStyle.None;

            Controls.Add(_logo);
            Controls.Add(_browser);
        }
    }
}
