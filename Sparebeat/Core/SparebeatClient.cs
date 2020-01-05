using Sparebeat.Common;
using Sparebeat.Utilities;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sparebeat.Core
{
    class SparebeatClient
    {
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
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<Beatmap> GetBeatmap(string id)
        {
            Beatmap beatmap = null;
            bool useCache = !string.IsNullOrEmpty(CacheDirectory);

            if (useCache)
                beatmap = GetBeatmapFromCache(id);

            if (beatmap == null)
            {
                beatmap = await GetBeatmapFromHttp(id);

                if (useCache && beatmap != null)
                    WriteBeatmapCache(beatmap);
            }

            return beatmap;
        }

        private async Task<Beatmap> GetBeatmapFromHttp(string id)
        {
            var mapTask = GetAsync<BeatmapMetadata>($"/play/{id}/map");
            var musicTask = GetAsync<byte[]>($"/play/{id}/music");

            await Task.WhenAll(mapTask, musicTask);

            if (mapTask.Result == null || musicTask.Result == null)
                return null;

            return new Beatmap
            {
                Metadata = mapTask.Result,
                Music = musicTask.Result
            };
        }

        private Beatmap GetBeatmapFromCache(string id)
        {
            var mapFile = Path.Combine(CacheDirectory, id, map);
            var musicFile = Path.Combine(CacheDirectory, id, music);

            if (!File.Exists(mapFile) || !File.Exists(musicFile))
                return null;

            string mapJson = File.ReadAllText(mapFile);

            return new Beatmap
            {
                Metadata = JsonSerializer.Deserialize<BeatmapMetadata>(mapJson, _serializerOptions),
                Music = File.ReadAllBytes(musicFile)
            };
        }

        private void WriteBeatmapCache(Beatmap beatmap)
        {
            var directory = Path.Combine(CacheDirectory, beatmap.Metadata.Id);
            var mapFile = Path.Combine(directory, map);
            var musicFile = Path.Combine(directory, music);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string mapJson = JsonSerializer.Serialize(beatmap.Metadata, _serializerOptions);

            File.WriteAllText(mapFile, mapJson);
            File.WriteAllBytes(musicFile, beatmap.Music);
        }

        private async Task<T> GetAsync<T>(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return default;

            if (typeof(T) == typeof(byte[]))
            {
                return (T)(object)await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json, _serializerOptions);
            }
        }
    }
}
