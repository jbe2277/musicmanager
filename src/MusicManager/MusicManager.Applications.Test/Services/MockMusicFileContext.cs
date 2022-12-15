using System.ComponentModel.Composition;
using Test.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Domain.MusicFiles;

namespace Test.MusicManager.Applications.Services;

[Export, Export(typeof(IMusicFileContext))]
public class MockMusicFileContext : IMusicFileContext
{
    private readonly Dictionary<string, MusicFile> musicFilesCache = new();

    public Action<MusicFile>? ApplyChangesAction { get; set; }

    public Func<MusicFile, Task>? SaveChangesAsyncAction { get; set; }

    public MusicFile Create(string fileName)
    {
        if (!musicFilesCache.TryGetValue(fileName, out var musicFile))
        {
            musicFile = MockMusicFile.CreateEmpty(fileName);
            musicFilesCache.Add(fileName, musicFile);
        }
        return musicFile;
    }

    public MusicFile CreateFromMultiple(IReadOnlyList<MusicFile> musicFiles)
    {
        return new MockMusicFile(new MusicMetadata(new TimeSpan(0, 3, 33), 320000), null) { SharedMusicFiles = musicFiles.ToArray() };
    }

    public void ApplyChanges(MusicFile musicFile) => ApplyChangesAction?.Invoke(musicFile);

    public Task SaveChangesAsync(MusicFile musicFile) => SaveChangesAsyncAction?.Invoke(musicFile) ?? Task.CompletedTask;
}
