using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.Services;

public interface IMusicFileContext
{
    MusicFile Create(string fileName);

    MusicFile CreateFromMultiple(IReadOnlyList<MusicFile> musicFiles);

    void ApplyChanges(MusicFile musicFile);

    Task SaveChangesAsync(MusicFile musicFile);
}
