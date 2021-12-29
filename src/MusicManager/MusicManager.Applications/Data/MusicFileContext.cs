using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Domain;
using Waf.MusicManager.Domain.MusicFiles;
using Windows.Storage;

namespace Waf.MusicManager.Applications.Data;

[Export, Export(typeof(IMusicFileContext)), PartMetadata(UnitTestMetadata.Name, UnitTestMetadata.Data)]
internal class MusicFileContext : IMusicFileContext
{
    private readonly ConcurrentDictionary<string, WeakReference<MusicFile>> musicFilesCache;
    private readonly ConcurrentDictionary<string, Task> runningTranscodingTasks;
    private readonly Stopwatch stopwatch;

    [ImportingConstructor]
    public MusicFileContext(IFileSystemWatcherService fileSystemWatcherService, ITranscodingService transcodingService)
    {
        musicFilesCache = new ConcurrentDictionary<string, WeakReference<MusicFile>>();
        runningTranscodingTasks = new ConcurrentDictionary<string, Task>();
        stopwatch = Stopwatch.StartNew();

        fileSystemWatcherService.Renamed += FileSystemWatcherServiceRenamed;
        fileSystemWatcherService.Deleted += FileSystemWatcherServiceDeleted;

        transcodingService.TranscodingTaskCreated += TranscodingServiceTranscodingTaskCreated;
    }

    public MusicFile Create(string fileName)
    {
        CompactCache();

        if (musicFilesCache.TryGetValue(fileName, out var weakMusicFile))
        {
            if (weakMusicFile.TryGetTarget(out var x)) return x;
            else musicFilesCache.TryRemove(fileName, out weakMusicFile);
        }

        runningTranscodingTasks.TryGetValue(fileName, out var runningTranscodingTask);
        var musicFile = new MusicFile(x => LoadMetadata(x ?? throw new InvalidOperationException("MusicFile does not contain a file name"), runningTranscodingTask)!, fileName);

        if (!musicFilesCache.TryAdd(fileName, new WeakReference<MusicFile>(musicFile))) throw new InvalidOperationException("Race condition: This should not happen.");
        return musicFile;
    }

    private static async Task<MusicMetadata> LoadMetadata(string fileName, Task? runningTranscodingTask)
    {
        if (runningTranscodingTask != null) await runningTranscodingTask.ConfigureAwait(false);

        var file = await StorageFile.GetFileFromPathAsync(fileName).AsTask().ConfigureAwait(false);
        var musicProperties = await file.Properties.GetMusicPropertiesAsync().AsTask().ConfigureAwait(false);

        var readMetadata = SupportedFileTypes.GetReadMetadata(file.FileType);
        return await readMetadata.CreateMusicMetadata(musicProperties, CancellationToken.None);
    }

    public MusicFile CreateFromMultiple(IEnumerable<MusicFile> musicFiles)
    {
        if (!musicFiles.Any()) throw new ArgumentException("The collection must have at least one item.", nameof(musicFiles));
        var localMusicFiles = musicFiles.ToArray();
        return new MusicFile(x => LoadMetadataFromMultiple(localMusicFiles)!, null) { SharedMusicFiles = localMusicFiles };
    }

    private static async Task<MusicMetadata> LoadMetadataFromMultiple(IReadOnlyList<MusicFile> musicFiles)
    {
        // Ensure that the Metadata of all files are loaded
        await TaskUtility.WhenAllFast(musicFiles.Select(x => x.GetMetadataAsync()));

        var duration = GetSharedValueOrDefault(musicFiles, x => x.Duration);
        var bitrate = GetSharedValueOrDefault(musicFiles, x => x.Bitrate);
        return new MusicMetadata(duration, bitrate)
        {
            Title = GetSharedValueOrDefault(musicFiles, x => x.Title) ?? "",
            Artists = GetSharedValueOrDefault<IReadOnlyList<string>>(musicFiles, x => x.Artists, SequenceEqualityComparer<string>.Default) ?? Array.Empty<string>(),
            Rating = GetSharedValueOrDefault(musicFiles, x => x.Rating),
            Album = GetSharedValueOrDefault(musicFiles, x => x.Album) ?? "",
            TrackNumber = GetSharedValueOrDefault(musicFiles, x => x.TrackNumber),
            Year = GetSharedValueOrDefault(musicFiles, x => x.Year),
            Genre = GetSharedValueOrDefault<IReadOnlyList<string>>(musicFiles, x => x.Genre, SequenceEqualityComparer<string>.Default) ?? Array.Empty<string>(),
            AlbumArtist = GetSharedValueOrDefault(musicFiles, x => x.AlbumArtist) ?? "",
            Publisher = GetSharedValueOrDefault(musicFiles, x => x.Publisher) ?? "",
            Subtitle = GetSharedValueOrDefault(musicFiles, x => x.Subtitle) ?? "",
            Composers = GetSharedValueOrDefault<IReadOnlyList<string>>(musicFiles, x => x.Composers, SequenceEqualityComparer<string>.Default) ?? Array.Empty<string>(),
            Conductors = GetSharedValueOrDefault<IReadOnlyList<string>>(musicFiles, x => x.Conductors, SequenceEqualityComparer<string>.Default) ?? Array.Empty<string>()
        };
    }

    public void ApplyChanges(MusicFile musicFile)
    {
        if (musicFile == null || !musicFile.IsMetadataLoaded || !musicFile.Metadata.HasChanges || !musicFile.SharedMusicFiles.Any()) return;

        var metadata = musicFile.Metadata;
        var changedProperties = metadata.GetChanges();
        foreach (var sharedMetadata in musicFile.SharedMusicFiles.Select(x => GetMetadata(x)))
        {
            if (changedProperties.Contains(nameof(MusicMetadata.Artists))) { sharedMetadata.Artists = metadata.Artists; }
            if (changedProperties.Contains(nameof(MusicMetadata.Title))) { sharedMetadata.Title = metadata.Title; }
            if (changedProperties.Contains(nameof(MusicMetadata.Rating))) { sharedMetadata.Rating = metadata.Rating; }
            if (changedProperties.Contains(nameof(MusicMetadata.Album))) { sharedMetadata.Album = metadata.Album; }
            if (changedProperties.Contains(nameof(MusicMetadata.TrackNumber))) { sharedMetadata.TrackNumber = metadata.TrackNumber; }
            if (changedProperties.Contains(nameof(MusicMetadata.Year))) { sharedMetadata.Year = metadata.Year; }
            if (changedProperties.Contains(nameof(MusicMetadata.Genre))) { sharedMetadata.Genre = metadata.Genre; }
            if (changedProperties.Contains(nameof(MusicMetadata.AlbumArtist))) { sharedMetadata.AlbumArtist = metadata.AlbumArtist; }
            if (changedProperties.Contains(nameof(MusicMetadata.Publisher))) { sharedMetadata.Publisher = metadata.Publisher; }
            if (changedProperties.Contains(nameof(MusicMetadata.Subtitle))) { sharedMetadata.Subtitle = metadata.Subtitle; }
            if (changedProperties.Contains(nameof(MusicMetadata.Composers))) { sharedMetadata.Composers = metadata.Composers; }
            if (changedProperties.Contains(nameof(MusicMetadata.Conductors))) { sharedMetadata.Conductors = metadata.Conductors; }
        }

        static MusicMetadata GetMetadata(MusicFile x) => x.Metadata ?? throw new InvalidOperationException("SharedMusicFile metadata is not yet loaded.");
    }

    public Task SaveChangesAsync(MusicFile musicFile)
    {
        var saveMetadata = SupportedFileTypes.GetSaveMetadata(Path.GetExtension(musicFile.FileName) ?? throw new InvalidOperationException("MusicFile does not contain a file name"));
        return saveMetadata.SaveChangesAsync(musicFile);
    }

    private void CompactCache()
    {
        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        stopwatch.Restart();
        // Performance optimization: Do not compact the cache more often than every second.
        if (elapsedMilliseconds < 1000) return;

        foreach (var entry in musicFilesCache.ToArray())
        {
            if (!entry.Value.TryGetTarget(out _)) TryRemoveFromCache(entry.Key);
        }
    }

    private void FileSystemWatcherServiceRenamed(object sender, RenamedEventArgs e) => TryRemoveFromCache(e.OldFullPath);

    private void FileSystemWatcherServiceDeleted(object sender, FileSystemEventArgs e) => TryRemoveFromCache(e.FullPath);

    private async void TranscodingServiceTranscodingTaskCreated(object? sender, TranscodingTaskEventArgs e)
    {
        runningTranscodingTasks.TryAdd(e.FileName, e.TranscodingTask);
        try
        {
            await e.TranscodingTask.ConfigureAwait(false);
        }
        catch (Exception)
        {
            // Here we ignore that transcoding has failed.
        }
        finally
        {
            runningTranscodingTasks.TryRemove(e.FileName, out _);
        }
    }

    private bool TryRemoveFromCache(string fileName) => musicFilesCache.TryRemove(fileName, out _);

    private static T? GetSharedValueOrDefault<T>(IEnumerable<MusicFile> musicFiles, Func<MusicMetadata, T> getValue, IEqualityComparer<T>? comparer = null)
    {
        comparer ??= EqualityComparer<T>.Default;
        var firstMusicFile = musicFiles.FirstOrDefault();
        if (firstMusicFile == null) return default;
        T firstValue = getValue(GetMetadata(firstMusicFile));
        if (musicFiles.Skip(1).Any(x => !comparer.Equals(getValue(GetMetadata(x)), firstValue))) return default;
        return firstValue;

        static MusicMetadata GetMetadata(MusicFile x) => x.Metadata ?? throw new InvalidOperationException("MusicFile metadata is not yet loaded.");
    }
}
