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
using Windows.Storage.FileProperties;

namespace Waf.MusicManager.Applications.Data
{
    [Export, Export(typeof(IMusicFileContext)), PartMetadata(UnitTestMetadata.Name, UnitTestMetadata.Data)]
    internal class MusicFileContext : IMusicFileContext
    {
        private readonly ConcurrentDictionary<string, WeakReference<MusicFile>> musicFilesCache;
        private readonly ConcurrentDictionary<string, Task> runningTranscodingTasks;
        private readonly Stopwatch stopwatch;


        [ImportingConstructor]
        public MusicFileContext(IFileSystemWatcherService fileSystemWatcherService, ITranscodingService transcodingService)
        {
            this.musicFilesCache = new ConcurrentDictionary<string, WeakReference<MusicFile>>();
            this.runningTranscodingTasks = new ConcurrentDictionary<string, Task>();
            this.stopwatch = Stopwatch.StartNew();

            fileSystemWatcherService.Renamed += FileSystemWatcherServiceRenamed;
            fileSystemWatcherService.Deleted += FileSystemWatcherServiceDeleted;

            transcodingService.TranscodingTaskCreated += TranscodingServiceTranscodingTaskCreated;
        }


        public MusicFile Create(string fileName)
        {
            CompactCache();

            WeakReference<MusicFile> weakMusicFile;
            MusicFile musicFile;
            if (musicFilesCache.TryGetValue(fileName, out weakMusicFile))
            {
                if (weakMusicFile.TryGetTarget(out musicFile))
                {
                    return musicFile;
                }
                else
                {
                    musicFilesCache.TryRemove(fileName, out weakMusicFile);
                }
            }

            Task runningTranscodingTask; 
            runningTranscodingTasks.TryGetValue(fileName, out runningTranscodingTask);
            musicFile = new MusicFile(x => LoadMetadata(x, runningTranscodingTask), fileName);

            if (!musicFilesCache.TryAdd(fileName, new WeakReference<MusicFile>(musicFile)))
            {
                throw new InvalidOperationException("Race condition: This should not happen.");
            }
            return musicFile;
        }

        private static async Task<MusicMetadata> LoadMetadata(string fileName, Task runningTranscodingTask)
        {
            if (runningTranscodingTask != null)
            {
                await runningTranscodingTask.ConfigureAwait(false);
            }
            
            var file = await StorageFile.GetFileFromPathAsync(fileName).AsTask().ConfigureAwait(false);
            MusicProperties musicProperties = await file.Properties.GetMusicPropertiesAsync().AsTask().ConfigureAwait(false);

            var readMetadata = SupportedFileTypes.GetReadMetadata(file.FileType);
            var musicMetadata = await readMetadata.CreateMusicMetadata(musicProperties, CancellationToken.None);
            musicMetadata.ClearChanges();
            return musicMetadata;
        }

        public MusicFile CreateFromMultiple(IEnumerable<MusicFile> musicFiles)
        {
            if (musicFiles.Count() < 1) { throw new ArgumentException("The collection must have at least one item.", "musicFiles"); }

            var localMusicFiles = musicFiles.ToArray();
            return new MusicFile(x => LoadMetadataFromMultiple(localMusicFiles), null)
            {
                SharedMusicFiles = localMusicFiles
            };
        }

        private static async Task<MusicMetadata> LoadMetadataFromMultiple(IEnumerable<MusicFile> musicFiles)
        {
            // Ensure that the Metadata of all files are loaded
            var tasks = musicFiles.Select(x => x.GetMetadataAsync());
            await TaskHelper.WhenAllFast(tasks);

            var duration = GetSharedValueOrDefault(musicFiles, x => x.Metadata.Duration);
            var bitrate = GetSharedValueOrDefault(musicFiles, x => x.Metadata.Bitrate);        
            var metadata = new MusicMetadata(duration, bitrate)
            {
                Title = GetSharedValueOrDefault(musicFiles, x => x.Metadata.Title) ?? "",
                Artists = GetSharedValueOrDefault<IReadOnlyList<string>>(musicFiles, x => x.Metadata.Artists, SequenceEqualityComparer<string>.Default) ?? new string[0],
                Rating = GetSharedValueOrDefault(musicFiles, x => x.Metadata.Rating),
                Album = GetSharedValueOrDefault(musicFiles, x => x.Metadata.Album) ?? "",
                TrackNumber = GetSharedValueOrDefault(musicFiles, x => x.Metadata.TrackNumber),
                Year = GetSharedValueOrDefault(musicFiles, x => x.Metadata.Year),
                Genre = GetSharedValueOrDefault<IReadOnlyList<string>>(musicFiles, x => x.Metadata.Genre, SequenceEqualityComparer<string>.Default) ?? new string[0],
                AlbumArtist = GetSharedValueOrDefault(musicFiles, x => x.Metadata.AlbumArtist) ?? "",
                Publisher = GetSharedValueOrDefault(musicFiles, x => x.Metadata.Publisher) ?? "",
                Subtitle = GetSharedValueOrDefault(musicFiles, x => x.Metadata.Subtitle) ?? "",
                Composers = GetSharedValueOrDefault<IReadOnlyList<string>>(musicFiles, x => x.Metadata.Composers, SequenceEqualityComparer<string>.Default) ?? new string[0],
                Conductors = GetSharedValueOrDefault<IReadOnlyList<string>>(musicFiles, x => x.Metadata.Conductors, SequenceEqualityComparer<string>.Default) ?? new string[0]
            };

            metadata.ClearChanges();
            return metadata;
        }

        public void ApplyChanges(MusicFile musicFile)
        {
            if (musicFile == null || !musicFile.IsMetadataLoaded || !musicFile.Metadata.HasChanges || !musicFile.SharedMusicFiles.Any())
            {
                return;
            }

            var metadata = musicFile.Metadata;
            var changedProperties = metadata.GetChanges();
            foreach (var sharedMetadata in musicFile.SharedMusicFiles.Select(x => x.Metadata))
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
        }

        public Task SaveChangesAsync(MusicFile musicFile)
        {
            var saveMetadata = SupportedFileTypes.GetSaveMetadata(Path.GetExtension(musicFile.FileName));
            return saveMetadata.SaveChangesAsync(musicFile);
        }

        private void CompactCache()
        {
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();
            if (elapsedMilliseconds < 1000)
            {
                // Performance optimization: Do not compact the cache more often than every second.
                return;
            }

            foreach (var entry in musicFilesCache.ToArray())
            {
                MusicFile musicFile;
                if (!entry.Value.TryGetTarget(out musicFile))
                {
                    TryRemoveFromCache(entry.Key);
                }
            }
        }

        private void FileSystemWatcherServiceRenamed(object sender, RenamedEventArgs e)
        {
            TryRemoveFromCache(e.OldFullPath);
        }

        private void FileSystemWatcherServiceDeleted(object sender, FileSystemEventArgs e)
        {
            TryRemoveFromCache(e.FullPath);
        }

        private async void TranscodingServiceTranscodingTaskCreated(object sender, TranscodingTaskEventArgs e)
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
                Task task;
                runningTranscodingTasks.TryRemove(e.FileName, out task);
            }
        }

        private bool TryRemoveFromCache(string fileName)
        {
            WeakReference<MusicFile> weakMusicFile;
            return musicFilesCache.TryRemove(fileName, out weakMusicFile);
        }

        private static T GetSharedValueOrDefault<T>(IEnumerable<MusicFile> musicFiles, Func<MusicFile, T> getValue, IEqualityComparer<T> comparer = null)
        {
            comparer = comparer ?? EqualityComparer<T>.Default;

            MusicFile firstMusicFile = musicFiles.FirstOrDefault();
            if (firstMusicFile == null)
            {
                return default(T);
            }
            T firstValue = getValue(firstMusicFile);
            if (musicFiles.Skip(1).Any(x => !comparer.Equals(getValue(x), firstValue)))
            {
                return default(T);
            }
            return firstValue;
        }
    }
}
