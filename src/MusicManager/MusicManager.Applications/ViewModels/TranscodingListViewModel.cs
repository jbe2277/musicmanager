using System.Waf.Applications;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Transcoding;

namespace Waf.MusicManager.Applications.ViewModels;

public class TranscodingListViewModel(ITranscodingListView view, ITranscodingService transcodingService) : ViewModel<ITranscodingListView>(view)
{
    public ITranscodingService TranscodingService { get; } = transcodingService;

    public ObservableList<TranscodeItem> SelectedTranscodeItems { get; } = [];

    public TranscodingManager TranscodingManager { get; set => SetProperty(ref field, value); } = null!;

    public Action<int, IEnumerable<string>> InsertFilesAction { get; set; } = null!;

    public Action<int, IEnumerable<MusicFile>> InsertMusicFilesAction { get; set; } = null!;
}
