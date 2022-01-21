using Waf.MusicManager.Applications.DataModels;

namespace Waf.MusicManager.Applications.Services
{
    public interface IFileService
    {
        static IReadOnlyList<string> PlaylistFileExtensions { get; } = new string[] { ".m3u", ".wpl" };

        bool IsFileSupported(string fileName);

        bool DirectoryExists(string path);

        Task<string?> GetParentPath(string path);

        Task<FolderItem> GetFolderFromPath(string path);

        Task<IReadOnlyList<FolderItem>> GetSubFoldersFromPath(string path);

        Task<string> GetDisplayPath(string path);

        Task<IReadOnlyList<string>> GetFiles(string directory, bool deep, string userSearchFilter, string applicationSearchFilter, CancellationToken cancellation);

        Task DeleteFile(string fileName);

        Task<IReadOnlyList<string>> ReadPlaylist(string playlistFileName);

        Task SavePlaylist(string playlistFileName, IReadOnlyList<string> fileNames);
    }
}
