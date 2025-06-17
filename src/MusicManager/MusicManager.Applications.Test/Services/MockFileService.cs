using Waf.MusicManager.Applications.Services;

namespace Test.MusicManager.Applications.Services;

public class MockFileService : IFileService
{
    public bool DirectoryExists(string path) => true;

    public Task DeleteFile(string fileName) => throw new NotImplementedException();

    public Task<string> GetDisplayPath(string path) => Task.FromResult(path);

    public Func<string, bool, string, string, CancellationToken, Task<IReadOnlyList<string>>>? GetFilesStub { get; set; }
    public Task<IReadOnlyList<string>> GetFiles(string directory, bool deep, string userSearchFilter, string applicationSearchFilter, CancellationToken cancellation) 
            => GetFilesStub?.Invoke(directory, deep, userSearchFilter, applicationSearchFilter, cancellation) ?? Task.FromResult<IReadOnlyList<string>>([]);

    public Task<FolderItem> GetFolderFromPath(string path) => Task.FromResult(new FolderItem(path, path));

    public Task<string?> GetParentPath(string path) => throw new NotImplementedException();

    public Task<IReadOnlyList<FolderItem>> GetSubFoldersFromPath(string path) => Task.FromResult<IReadOnlyList<FolderItem>>([]);

    public bool IsFileSupported(string fileName) => true;

    public Task<IReadOnlyList<string>> ReadPlaylist(string playlistFileName) => throw new NotImplementedException();

    public Task SavePlaylist(string playlistFileName, IReadOnlyList<string> fileNames) => throw new NotImplementedException();
}
