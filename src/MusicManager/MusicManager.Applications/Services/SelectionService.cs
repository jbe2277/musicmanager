using System.Waf.Foundation;
using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.Services;

internal class SelectionService : ISelectionService
{
    public ObservableListViewCore<MusicFileDataModel> MusicFiles { get; private set; } = null!;

    public ObservableList<MusicFileDataModel> SelectedMusicFiles { get; } = [];

    public void Initialize(IEnumerable<MusicFile> musicFiles)
    {
        MusicFiles = new(new SynchronizingList<MusicFileDataModel, MusicFile>(musicFiles, x => new MusicFileDataModel(x)));
    }
}
