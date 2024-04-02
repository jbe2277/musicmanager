using Waf.MusicManager.Applications.DataModels;

namespace Waf.MusicManager.Applications.Services;

public interface ISelectionService
{
    ObservableListViewCore<MusicFileDataModel> MusicFiles { get; }

    ObservableList<MusicFileDataModel> SelectedMusicFiles { get; }
}
