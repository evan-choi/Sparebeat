﻿using CefSharp;
using Sparebeat.Common;
using Sparebeat.Json;
using Sparebeat.Utilities;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sparebeat.Core
{
    class SparebeatBrowser
    {
        public event EventHandler Loadded;
        public event EventHandler<Beatmap> BeatmapChanged;

        private IWebBrowser _browser;
        private ContextUtility.ContextSnapshot _snapshot;

        private readonly JsonSerializerOptions _serializerOptions;

        public bool IsLoaded => !_browser.IsLoading;

        public SparebeatBrowser(IWebBrowser webBrowser)
        {
            _snapshot = this.Snapshot();

            _browser = webBrowser;
            _browser.LoadingStateChanged += LoadingStateChanged;

            _serializerOptions = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new Int32Converter(),
                    new INoteConverter()
                }
            };
        }

        private void LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.Browser.IsLoading)
                return;

#if DEBUG
            _browser.ShowDevTools();
#endif

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

            if (response.Success)
                _snapshot.BeginInvokeEvent(BeatmapChanged, this, beatmap);

            return response.Success;
        }
    }
}
