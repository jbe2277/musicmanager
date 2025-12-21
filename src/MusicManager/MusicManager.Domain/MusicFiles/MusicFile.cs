namespace Waf.MusicManager.Domain.MusicFiles;

public class MusicFile(Func<string?, Task<MusicMetadata?>> loadMetadata, string? fileName) : Model
{
    private readonly TaskCompletionSource<MusicMetadata> loadMetadataCompletionSource = new();
    private readonly Func<string?, Task<MusicMetadata?>> loadMetadata = loadMetadata;
    private bool loadCalled;

    public string? FileName { get; } = fileName;

    public IReadOnlyList<MusicFile> SharedMusicFiles { get; set => SetProperty(ref field, value); } = [];

    public MusicMetadata? Metadata
    {
        get
        {
            if (field == null) LoadMetadataCore();
            return field;
        }
        private set => SetProperty(ref field, value);
    }

    [MemberNotNullWhen(true, nameof(Metadata))] public bool IsMetadataLoaded { get; private set => SetProperty(ref field, value); }

    public Exception? LoadError { get; private set => SetProperty(ref field, value); }

    public Task<MusicMetadata> GetMetadataAsync()
    {
        LoadMetadataCore();
        return loadMetadataCompletionSource.Task;
    }

    private async void LoadMetadataCore()
    {
        if (loadCalled) return;
        loadCalled = true;
        try
        {
            var musicMetadata = await loadMetadata(FileName);
            if (musicMetadata == null) throw new InvalidOperationException("The loadMetadata delegate must not return null.");
            musicMetadata.Parent = this;
            musicMetadata.EntityLoadCompleted();
            Metadata = musicMetadata;
            IsMetadataLoaded = true;
            loadMetadataCompletionSource.SetResult(Metadata);
            if (!string.IsNullOrEmpty(FileName)) Log.Default.Trace("MusicFile.MetadataLoaded: {0}", FileName);
        }
        catch (Exception e)
        {
            Log.Default.Error(e, "LoadMetadataCore");
            LoadError = e;
            // Observe the exception
            loadMetadataCompletionSource.Task.NoWait(ignoreExceptions: true);
            loadMetadataCompletionSource.SetException(e);
        }
    }
}
