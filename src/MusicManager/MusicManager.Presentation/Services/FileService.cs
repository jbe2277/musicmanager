using System.ComponentModel.Composition;
using System.IO;
using System.Web;
using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Applications.Services;
using Windows.Media.Playlists;
using Windows.Storage;
using Windows.Storage.Search;

namespace Waf.MusicManager.Presentation.Services
{
    [Export, Export(typeof(IFileService))]
    internal class FileService : IFileService
    {
        public bool IsFileSupported(string fileName) => SupportedFileTypes.MusicFileExtensions.Contains(Path.GetExtension(fileName));

        public bool DirectoryExists(string path) => Directory.Exists(path);

        public async Task<string?> GetParentPath(string path)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(path).AsTask().ConfigureAwait(false);
            var parent = await folder.GetParentAsync().AsTask().ConfigureAwait(false);
            return parent?.Path ?? "";
        }

        public async Task<FolderItem> GetFolderFromPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var folder = await GetFolderFromLocalizedPathAsync(path).ConfigureAwait(false);
                return new FolderItem(folder.Path, folder.DisplayName);
            }
            else
            {
                return new FolderItem(null, "Root");
            }
        }

        public async Task<IReadOnlyList<FolderItem>> GetSubFoldersFromPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var folder = await StorageFolder.GetFolderFromPathAsync(path).AsTask().ConfigureAwait(false);
                var result = folder.CreateFolderQuery();
                var folders = await result.GetFoldersAsync().AsTask().ConfigureAwait(false);
                return folders.Select(x => new FolderItem(x.Path, x.DisplayName)).ToArray();
            }
            else
            {
                var driveInfos = DriveInfo.GetDrives();
                return await Task.WhenAll(driveInfos.Select(async x =>
                {
                    var displayName = (await StorageFolder.GetFolderFromPathAsync(x.RootDirectory.FullName).AsTask().ConfigureAwait(false)).DisplayName;
                    return new FolderItem(x.RootDirectory.FullName, displayName);
                })).ConfigureAwait(false);
            }
        }

        public async Task<string> GetDisplayPath(string path)
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

        public async Task<IReadOnlyList<string>> GetFiles(string directory, bool deep, string userSearchFilter, string applicationSearchFilter, CancellationToken cancellation)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(directory).AsTask(cancellation);  // Not allowed to use .ConfigureAwait(false) -> see https://github.com/microsoft/CsWinRT/issues/1112
            var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, SupportedFileTypes.MusicFileExtensions)
            {
                UserSearchFilter = userSearchFilter ?? "",
                ApplicationSearchFilter = applicationSearchFilter ?? "",
                FolderDepth = deep ? FolderDepth.Deep : FolderDepth.Shallow
            };
            var result = folder.CreateFileQueryWithOptions(queryOptions);

            // It seems that GetFilesAsync does not check the cancellationToken; so get only parts of the file results in a loop.
            Log.Default.Trace("ManagerController.UpdateMusicFiles:GetFilesAsync Start");
            var files = new List<string>();
            uint index = 0;
            int resultCount;
            const uint maxFiles = 100;
            do
            {
                var filesResult = await result.GetFilesAsync(index, maxFiles).AsTask(cancellation).ConfigureAwait(false);
                resultCount = filesResult.Count;
                files.AddRange(filesResult.Select(x => x.Path));
                index += maxFiles;
            }
            while (resultCount == maxFiles);
            Log.Default.Trace("ManagerController.UpdateMusicFiles:GetFilesAsync End");
            return files;
        }

        public async Task DeleteFile(string fileName)
        {
            var file = await StorageFile.GetFileFromPathAsync(fileName).AsTask().ConfigureAwait(false);
            await file.DeleteAsync().AsTask().ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<string>> ReadPlaylist(string playlistFileName)
        {
            var playlistFile = await StorageFile.GetFileFromPathAsync(playlistFileName).AsTask().ConfigureAwait(false);
            // MS Issue: LoadAsync cannot load a playlist when one of the files do not exists anymore.
            var playlist = await Playlist.LoadAsync(playlistFile).AsTask().ConfigureAwait(false);
            return playlist.Files.Select(x => HttpUtility.UrlDecode(x.Path)).ToArray();
        }

        public async Task SavePlaylist(string playlistFileName, IReadOnlyList<string> fileNames)
        {
            var playlist = new Playlist();
            foreach (var fileName in fileNames)
            {
                var file = await StorageFile.GetFileFromPathAsync(fileName).AsTask().ConfigureAwait(false);
                playlist.Files.Add(file);
            }
            var targetFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(playlistFileName)).AsTask().ConfigureAwait(false);
            var name = Path.GetFileNameWithoutExtension(playlistFileName);
            await playlist.SaveAsAsync(targetFolder, name, NameCollisionOption.ReplaceExisting, PlaylistFormat.M3u).AsTask().ConfigureAwait(false);
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

        internal static async Task<StorageFolder> GetFolderFromLocalizedPathAsync(string path)
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
    }
}
