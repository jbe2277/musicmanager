using System.IO;
using Windows.Storage;

namespace Waf.MusicManager.Applications.Data;

public static class FolderHelper
{
    public static async Task<StorageFolder> GetFolderFromLocalizedPathAsync(string path)
    {
        string? corePath = null;
        try
        {
            // Try to parse a user-friendly (localized) path.
            var pathSegments = GetPathSegments(path);
            corePath = pathSegments[0];
            foreach (var pathSegment in pathSegments.Skip(1))
            {
                var folder = await StorageFolder.GetFolderFromPathAsync(corePath).AsTask().ConfigureAwait(false);
                var subFolders = await folder.GetFoldersAsync().AsTask().ConfigureAwait(false);
                var foundFolder = subFolders.FirstOrDefault(x =>
                    pathSegment.Equals(x.Name, StringComparison.OrdinalIgnoreCase) || pathSegment.Equals(x.DisplayName, StringComparison.OrdinalIgnoreCase));
                if (foundFolder == null)
                {
                    corePath = null;
                    break;
                }
                corePath = foundFolder.Path;
            }
        }
        catch (Exception)
        {
            corePath = null;
        }
        return await StorageFolder.GetFolderFromPathAsync(corePath ?? path).AsTask().ConfigureAwait(false);
    }

    public static async Task<string> GetDisplayPath(string path)
    {
        string? displayPath;
        try
        {
            var pathSegments = GetPathSegments(path);
            displayPath = pathSegments[0];
            var currentPath = pathSegments[0];
            foreach (var pathSegment in pathSegments.Skip(1))
            {
                currentPath = Path.Combine(currentPath, pathSegment);
                var folder = await StorageFolder.GetFolderFromPathAsync(currentPath).AsTask().ConfigureAwait(false);
                displayPath = Path.Combine(displayPath, folder.DisplayName);
            }
        }
        catch (Exception)
        {
            displayPath = null;
        }
        return displayPath ?? path;
    }

    public static IReadOnlyList<string> GetPathSegments(string path)
    {
        var root = Path.GetPathRoot(path);
        if (string.IsNullOrEmpty(root)) return Array.Empty<string>();
        var innerPath = path[root.Length..];
        innerPath = innerPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        var pathSegments = new[] { root }.Concat(innerPath.Split(Path.DirectorySeparatorChar).Where(x => !string.IsNullOrEmpty(x))).ToArray();
        return pathSegments;
    }
}
