using System;
using System.ComponentModel.Composition;
using System.Waf.UnitTesting.Mocks;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.Playlists;

namespace Test.MusicManager.Applications.Views
{
    [Export, Export(typeof(IPlaylistView))]
    public class MockPlaylistView : MockView, IPlaylistView
    {
        public Action FocusSearchBoxStub { get; set; }
        
        public Action FocusSelectedItemStub { get; set; }

        public Action<PlaylistItem> ScrollIntoViewStub { get; set; }
        
        
        public void FocusSearchBox()
        {
            if (FocusSearchBoxStub != null) { FocusSearchBoxStub(); }
        }
        
        public void FocusSelectedItem()
        {
            if (FocusSelectedItemStub != null) { FocusSelectedItemStub(); }
        }

        public void ScrollIntoView(PlaylistItem item)
        {
            if (ScrollIntoViewStub != null) { ScrollIntoViewStub(item); }
        }
    }
}
