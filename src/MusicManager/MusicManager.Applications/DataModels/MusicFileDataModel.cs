using System.Globalization;
using System.Waf.Foundation;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.DataModels;

public class MusicFileDataModel : Model
{
    private IWeakEventProxy? musicFilePropertyChangedProxy;

    public MusicFileDataModel(MusicFile musicFile)
    {
        MusicFile = musicFile;
        if (musicFile.IsMetadataLoaded)
        {
            MetadataLoaded();
        }
        else
        {
            musicFilePropertyChangedProxy = WeakEvent.PropertyChanged.Add(musicFile, MusicFilePropertyChanged);
        }
    }

    public MusicFile MusicFile { get; }

    public string ArtistsString => string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator + " ", MusicFile.IsMetadataLoaded ? MusicFile.Metadata.Artists : []);

    private void MetadataLoaded() => PropertyChangedEventManager.AddHandler(MusicFile.Metadata, MetadataPropertyChanged, "");

    private void MusicFilePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MusicFile.IsMetadataLoaded))
        {
            WeakEvent.TryRemove(ref musicFilePropertyChangedProxy);
            MetadataLoaded();
            RaisePropertyChanged(nameof(ArtistsString));
        }
    }

    private void MetadataPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MusicMetadata.Artists)) RaisePropertyChanged(nameof(ArtistsString));
    }
}
