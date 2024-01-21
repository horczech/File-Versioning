using System.Runtime.InteropServices;

namespace PuxTask.Server.Application.Services;

public static class PathComparer
{
    public static readonly StringComparison ComparisonType = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? StringComparison.OrdinalIgnoreCase //windows is case invariant
        : StringComparison.Ordinal; //linux is case sensitive

    public static bool Equals(string path1, string path2) {
        return string.Equals(Path.GetFullPath(path1), Path.GetFullPath(path2), ComparisonType); //Note: works on both linux and windows
    }
}