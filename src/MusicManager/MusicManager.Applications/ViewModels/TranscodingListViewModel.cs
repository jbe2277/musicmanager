using System.Waf.Applications;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Transcoding;

namespace Waf.MusicManager.Applications.ViewModels;

public class TranscodingListViewModel : ViewModel<ITranscodingListView>
{
    private TranscodingManager transcodingManager = null!;

    public TranscodingListViewModel(ITranscodingListView view, ITranscodingService transcodingService) : base(view)
    {
        TranscodingService = transcodingService;
        SelectedTranscodeItems = [];
    }

    public ITranscodingService TranscodingService { get; }

    public ObservableList<TranscodeItem> SelectedTranscodeItems { get; }

    public TranscodingManager TranscodingManager
    {
        get => transcodingManager;
        set => SetProperty(ref transcodingManager, value);
    }

    public Action<int, IEnumerable<string>> InsertFilesAction { get; set; } = null!;

    public Action<int, IEnumerable<MusicFile>> InsertMusicFilesAction { get; set; } = null!;
}
