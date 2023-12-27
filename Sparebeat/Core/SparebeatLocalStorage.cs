using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using Sparebeat.Common;
using Sparebeat.Handler;
using BrowserSettings = CefSharp.Core.BrowserSettings;

namespace Sparebeat.Core;

internal sealed class SparebeatLocalStorage
{
    private readonly ChromiumWebBrowser _browser;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public SparebeatLocalStorage()
    {
        _browser = new ChromiumWebBrowser();
        _browser.Size = Size.Empty;
        _browser.ResourceRequestHandlerFactory = new BridgeHandlerFactory();
        SpinWait.SpinUntil(() => _browser.IsBrowserInitialized);
        _browser.Load("app://main.html");
        SpinWait.SpinUntil(() => !_browser.IsLoading);
    }

    public async Task<FrozenDictionary<string, RecordSet>> GetRecordsAsync()
    {
        var json = await EvalAsync<string>("localStorage.records");

        return string.IsNullOrEmpty(json)
            ? FrozenDictionary<string, RecordSet>.Empty
            : JsonSerializer.Deserialize<Dictionary<string, RecordSet>>(json, _jsonSerializerOptions).ToFrozenDictionary();
    }

    public async Task<RecordSet> FindRecordAsync(string id)
    {
        var script =
            $$"""
              if (localStorage.records) {
                  JSON.stringify(JSON.parse(localStorage.records)['{{id}}'])
              } else {
                  null;
              }
              """;

        var json = await EvalAsync<string>(script);

        return string.IsNullOrEmpty(json)
            ? null
            : JsonSerializer.Deserialize<RecordSet>(json, _jsonSerializerOptions);
    }

    private async Task<T> EvalAsync<T>(string script)
    {
        script = JsonSerializer.Serialize(script);
        var response = await _browser.EvaluateScriptAsPromiseAsync($"return Sparebeat.eval({script});");

        if (response.Success)
            return (T)response.Result;

        throw new Exception(response.Message);
    }
}
