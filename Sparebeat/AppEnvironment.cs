using System;
using System.IO;

namespace Sparebeat;

internal static class AppEnvironment
{
    private const string songsDir = "Songs";
    private const string cacheDir = "Cache";

#if DEBUG
        private static readonly string _storage = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Sparebeat(Development)";
#else
    private static readonly string _storage = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Sparebeat";
#endif

    public static string Storage
    {
        get
        {
            if (!Directory.Exists(_storage))
                Directory.CreateDirectory(_storage);

            return _storage;
        }
    }

    public static string Songs
    {
        get
        {
            var combine = Path.Combine(Storage, songsDir);

            if (!Directory.Exists(combine))
                Directory.CreateDirectory(combine);

            return combine;
        }
    }

    public static string Cache
    {
        get
        {
            var combine = Path.Combine(Storage, cacheDir);

            if (!Directory.Exists(combine))
                Directory.CreateDirectory(combine);

            return combine;
        }
    }
}