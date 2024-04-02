using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Applications.Services;

namespace Waf.MusicManager.Presentation.DesignData;

public class MockSelectionService : ISelectionService
{
    private readonly ObservableCollection<MusicFileDataModel> innerMusicFiles;

    public MockSelectionService()
    {
        innerMusicFiles = [];
        SelectedMusicFiles = [];
        MusicFiles = new(innerMusicFiles);
    }

    public ObservableListViewCore<MusicFileDataModel> MusicFiles { get; }

    public ObservableList<MusicFileDataModel> SelectedMusicFiles { get; }
        
    public void SetMusicFiles(IEnumerable<MusicFileDataModel> musicFiles)
    {
        innerMusicFiles.Clear();
        musicFiles.ToList().ForEach(x => innerMusicFiles.Add(x));
    }
}
