using System.ComponentModel.Composition;
using System.Waf.Applications;
using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.Services;

[Export, Export(typeof(ISelectionService))]
internal class SelectionService : ISelectionService
{
    public ObservableListView<MusicFileDataModel> MusicFiles { get; private set; } = null!;

    public ObservableList<MusicFileDataModel> SelectedMusicFiles { get; } = [];

    public void Initialize(IEnumerable<MusicFile> musicFiles)
    {
        MusicFiles = new ObservableListView<MusicFileDataModel>(new SynchronizingList<MusicFileDataModel, MusicFile>(musicFiles, x => new MusicFileDataModel(x)));
    }
}
