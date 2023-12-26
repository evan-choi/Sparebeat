using HtmlAgilityPack;
using Sparebeat.Common;
using Sparebeat.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sparebeat.Core;

internal partial class SparebeatClient
{
    [GeneratedRegex(@"\d+")]
    private static partial Regex DigitsRegex();

    private const string map = "map";
    private const string music = "music";

    public string CacheDirectory { get; set; }

    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _serializerOptions;

    public SparebeatClient(string cacheDirectory = null)
    {
        CacheDirectory = cacheDirectory;

        _client = new HttpClient
        {
            BaseAddress = new Uri("https://sparebeat.com")
        };

        var userAgent = $"Sparebeat/{AssemblyUtility.GetVersion()} ({RuntimeInformation.OSDescription})";
        _client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);

        _serializerOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<BeatmapInfo[]> GetBeatmapInfos()
    {
        var result = new List<BeatmapInfo>();

        var htmlStream = await GetAsync<Stream>("/");
        var document = new HtmlDocument();
        document.Load(htmlStream);

        var items = document.DocumentNode.SelectNodes("//li[contains(@class, 'music-list-item')]");

        foreach (var item in items)
        {
            var id = item.GetAttributeValue("id", null);
            var title = item.SelectSingleNode("div/div[contains(@class, 'music-list-item-title')]").InnerText.Normalize();
            var artist = item.SelectSingleNode("div/div[contains(@class, 'music-list-item-artist')]").InnerText.Normalize();
            var levelText = item.SelectSingleNode("div[contains(@class, 'music-list-item-sub')]").InnerText.Normalize();
            var scoreText = item.SelectSingleNode("div[contains(@class, 'music-list-item-score')]").InnerText.Normalize();

            var levels = DigitsRegex().Matches(levelText)
                .Select(m => int.Parse(m.Value))
                .ToArray();

            result.Add(new BeatmapInfo
            {
                Id = id,
                Title = title,
                Artist = artist,
                Level = new BeatmapLevelSet
                {
                    Easy = levels[0],
                    Normal = levels[1],
                    Hard = levels[2]
                },
                Score = int.Parse(scoreText)
            });
        }

        return result.ToArray();
    }

    public async Task<Beatmap> GetBeatmap(BeatmapInfo info)
    {
        Beatmap beatmap = null;
        bool useCache = !string.IsNullOrEmpty(CacheDirectory);

        if (useCache)
            beatmap = GetBeatmapFromCache(info);

        if (beatmap == null)
        {
            beatmap = await GetBeatmapFromHttp(info);

            if (useCache && beatmap != null)
                WriteBeatmapCache(info, beatmap);
        }

        return beatmap;
    }

    private async Task<Beatmap> GetBeatmapFromHttp(BeatmapInfo info)
    {
        var mapTask = GetAsync<string>($"/play/{info.Id}/map");
        var musicTask = GetAsync<byte[]>($"/play/{info.Id}/music");

        await Task.WhenAll(mapTask, musicTask);

        if (mapTask.Result == null || musicTask.Result == null)
            return null;

        return new Beatmap
        {
            MapJson = mapTask.Result,
            Music = musicTask.Result
        };
    }

    private Beatmap GetBeatmapFromCache(BeatmapInfo info)
    {
        var mapFile = Path.Combine(CacheDirectory, info.Id, map);
        var musicFile = Path.Combine(CacheDirectory, info.Id, music);

        if (!File.Exists(mapFile) || !File.Exists(musicFile))
            return null;

        string mapJson = File.ReadAllText(mapFile);

        return new Beatmap
        {
            MapJson = mapJson,
            Music = File.ReadAllBytes(musicFile)
        };
    }

    private void WriteBeatmapCache(BeatmapInfo info, Beatmap beatmap)
    {
        var directory = Path.Combine(CacheDirectory, info.Id);
        var mapFile = Path.Combine(directory, map);
        var musicFile = Path.Combine(directory, music);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(mapFile, beatmap.MapJson);
        File.WriteAllBytes(musicFile, beatmap.Music);
    }

    private async Task<T> GetAsync<T>(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        using var response = await _client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return default;

        object result;

        switch (typeof(T))
        {
            case var t when t == typeof(byte[]):
                result = await response.Content.ReadAsByteArrayAsync();
                break;

            case var t when t == typeof(Stream):
                result = new MemoryStream(await response.Content.ReadAsByteArrayAsync());
                break;

            case var t when t == typeof(string):
                result = await response.Content.ReadAsStringAsync();
                break;

            default:
                string json = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<T>(json, _serializerOptions);
                break;
        }

        return (T)result;
    }
}
