using System.Waf.Applications;
using Waf.MusicManager.Applications.DataModels;

namespace Waf.MusicManager.Applications.Services;

public interface ISelectionService
{
    ObservableListView<MusicFileDataModel> MusicFiles { get; }

    ObservableList<MusicFileDataModel> SelectedMusicFiles { get; }
}
