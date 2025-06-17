using System.Waf.UnitTesting.Mocks;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.Playlists;

namespace Test.MusicManager.Applications.Views;

public class MockPlaylistView : MockView, IPlaylistView
{
    public Action? FocusSearchBoxStub { get; set; }
        
    public Action? FocusSelectedItemStub { get; set; }

    public Action<PlaylistItem>? ScrollIntoViewStub { get; set; }

    public void FocusSearchBox() => FocusSearchBoxStub?.Invoke();

    public void FocusSelectedItem() => FocusSelectedItemStub?.Invoke();

    public void ScrollIntoView(PlaylistItem item) => ScrollIntoViewStub?.Invoke(item);
}
