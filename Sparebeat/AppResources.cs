using System;
using System.Collections.Frozen;
using System.IO;
using System.Reflection;

namespace Sparebeat;

public static class AppResources
{
    private static readonly Assembly _assembly;
    private static readonly FrozenSet<string> _resourceNames;

    static AppResources()
    {
        _assembly = Assembly.GetExecutingAssembly();
        _resourceNames = _assembly.GetManifestResourceNames().ToFrozenSet();
    }

    public static Stream GetStream(string path)
    {
        if (TryResolvePath(path, out var resourcePath))
            return _assembly.GetManifestResourceStream(resourcePath);

        throw new ArgumentException($"{path} not found", nameof(path));
    }

    public static bool Exists(string path)
    {
        return _resourceNames.Contains(path);
    }

    public static bool TryGetResourceStream(string url, out Stream resourceStream)
    {
        if (TryResolvePath(url, out var resourcePath))
        {
            resourceStream = GetStream(resourcePath);
            return true;
        }

        resourceStream = default;
        return false;
    }

    public static bool TryResolvePath(string url, out string resourcePath)
    {
        const string schemePrefix = "app://";
        const string resourcePrefix = "Sparebeat.Resources.App.";

        if (url.StartsWith(resourcePrefix))
        {
            resourcePath = url;
        }
        else if (url.StartsWith(schemePrefix))
        {
            var name = url[schemePrefix.Length..].TrimEnd('/').Replace('/', '.');
            resourcePath = resourcePrefix + name;
        }
        else
        {
            resourcePath = default;
            return false;
        }

        if (!Exists(resourcePath))
        {
            resourcePath = default;
            return false;
        }

        return true;
    }
}
