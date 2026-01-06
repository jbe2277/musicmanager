using System.Waf.Applications;
using System.Waf.Foundation;
using System.Windows.Input;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Playlists;

namespace Waf.MusicManager.Applications.ViewModels;

public class PlaylistViewModel : ViewModel<IPlaylistView>
{
    public PlaylistViewModel(IPlaylistView view) : base(view)
    {
        SearchNextCommand = new DelegateCommand(SearchNext);
        SearchPreviousCommand = new DelegateCommand(SearchPrevious);
        ClearSearchCommand = new DelegateCommand(ClearSearch);
    }

    public PlaylistManager PlaylistManager { get; set => SetProperty(ref field, value); } = null!;

    public PlaylistItem? SelectedPlaylistItem { get; set => SetProperty(ref field, value); }

    public ObservableList<PlaylistItem> SelectedPlaylistItems { get; } = [];

    public ICommand PlaySelectedCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand RemoveSelectedCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand ShowMusicPropertiesCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand OpenListCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand SaveListCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand ClearListCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public Action<int, IEnumerable<string>> InsertFilesAction { get; set; } = null!;

    public Action<int, IEnumerable<MusicFile>> InsertMusicFilesAction { get; set; } = null!;

    public ICommand SearchNextCommand { get; }

    public ICommand SearchPreviousCommand { get; }

    public ICommand ClearSearchCommand { get; }

    public string? SearchText
    {
        get;
        set { if (SetProperty(ref field, value)) SearchTextCore(SearchMode.Default); }
    }

    private void SearchTextCore(SearchMode searchMode)
    {
        if (!string.IsNullOrEmpty(SearchText))
        {
            IEnumerable<PlaylistItem> itemsToSearch;
            if (SelectedPlaylistItem != null)
            {
                var index = PlaylistManager.Items.IndexOf(SelectedPlaylistItem);
                if (searchMode == SearchMode.Next) index++;  // Skip the current item so that the next one will be found.
                itemsToSearch = PlaylistManager.Items.Skip(index).Concat(PlaylistManager.Items.Take(index));
            }
            else
            {
                itemsToSearch = PlaylistManager.Items;
            }

            if (searchMode == SearchMode.Previous) itemsToSearch = itemsToSearch.Reverse();

            var foundItem = itemsToSearch.FirstOrDefault(x => IsContained(x.MusicFile, SearchText));
            if (foundItem != null)
            {
                SelectedPlaylistItem = foundItem;
                ViewCore.ScrollIntoView(foundItem);
            }
        }
        ViewCore.FocusSearchBox();
    }

    public void FocusSelectedItem() => ViewCore.FocusSelectedItem();

    public void ScrollIntoView()
    {
        if (PlaylistManager.CurrentItem != null) ViewCore.ScrollIntoView(PlaylistManager.CurrentItem);
    }

    private void SearchNext() => SearchTextCore(SearchMode.Next);

    private void SearchPrevious() => SearchTextCore(SearchMode.Previous);

    private void ClearSearch() => SearchText = "";

    private static bool IsContained(MusicFile musicFile, string searchText)
    {
        return MusicTitleHelper.GetTitleText(musicFile.FileName, musicFile.IsMetadataLoaded ? musicFile.Metadata.Artists : null, musicFile.IsMetadataLoaded ? musicFile.Metadata.Title : null)
                .Contains(searchText, StringComparison.CurrentCultureIgnoreCase)
            || musicFile.IsMetadataLoaded && musicFile.Metadata.Artists.Any(y => y.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));
    }

    private enum SearchMode
    {
        Default,
        Next,
        Previous
    }
}
