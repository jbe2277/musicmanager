using System.Waf.Applications;
using System.Windows.Input;
using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.Views;

namespace Waf.MusicManager.Applications.ViewModels;

public class ManagerViewModel : ViewModel<IManagerView>
{
    private readonly Lazy<ISelectionService> selectionService;

    public ManagerViewModel(IManagerView view, Lazy<ISelectionService> selectionService, IManagerStatusService managerStatusService, IPlayerService playerService, ITranscodingService transcodingService)
        : base(view)
    {
        this.selectionService = selectionService;
        ManagerStatusService = managerStatusService;
        PlayerService = playerService;
        TranscodingService = transcodingService;
        ClearSearchCommand = new DelegateCommand(ClearSearch);
    }

    public ISelectionService SelectionService => selectionService.Value;

    public IManagerStatusService ManagerStatusService { get; }

    public IPlayerService PlayerService { get; }

    public ITranscodingService TranscodingService { get; }

    public FolderBrowserDataModel FolderBrowser { get; } = new();

    public SearchFilterDataModel SearchFilter { get; } = new();

    public MusicFileDataModel? SelectedMusicFile { get; set => SetProperty(ref field, value); }

    public ICommand UpdateSubDirectoriesCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand NavigateDirectoryUpCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand NavigateHomeCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand NavigatePublicHomeCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand LoadRecursiveCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand NavigateToSelectedSubDirectoryCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand ClearSearchCommand { get; }

    public ICommand ShowMusicPropertiesCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand DeleteSelectedFilesCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    private void ClearSearch() => SearchFilter.Clear();
}
