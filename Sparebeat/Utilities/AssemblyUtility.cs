using System.Diagnostics;
using System.Reflection;

namespace Sparebeat.Utilities;

internal static class AssemblyUtility
{
    public static string GetVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

        return versionInfo.FileVersion;
    }
}