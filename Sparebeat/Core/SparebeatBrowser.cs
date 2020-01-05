using CefSharp;
using Sparebeat.Common;
using Sparebeat.Utilities;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sparebeat.Core
{
    class SparebeatBrowser
    {
        public event EventHandler Loadded;

        private IWebBrowser _browser;
        private ContextUtility.ContextSnapshot _snapshot;

        private readonly JsonSerializerOptions _serializerOptions;

        public SparebeatBrowser(IWebBrowser webBrowser)
        {
            _snapshot = this.Snapshot();

            _browser = webBrowser;
            _browser.LoadingStateChanged += LoadingStateChanged;

            _serializerOptions = new JsonSerializerOptions
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        private void LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.Browser.IsLoading)
                return;

            _browser.LoadingStateChanged -= LoadingStateChanged;
            OnPageLoadded();
        }

        private void OnPageLoadded()
        {
            _snapshot.InvokeEvent(Loadded, this, EventArgs.Empty);
        }

        public async Task<bool> Load(Beatmap beatmap)
        {
            if (beatmap == null)
                throw new ArgumentNullException();

            string mapJson = JsonSerializer.Serialize(beatmap.Metadata, _serializerOptions);
            string musicBin = Convert.ToBase64String(beatmap.Music);

            string script = $"Sparebeat.load({mapJson}, '{musicBin}')";
            var response = await _browser.EvaluateScriptAsync(script);

            return response.Success;
        }
    }
}
