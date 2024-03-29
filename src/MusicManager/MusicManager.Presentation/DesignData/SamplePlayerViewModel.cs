﻿using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Playlists;

namespace Waf.MusicManager.Presentation.DesignData;

public class SamplePlayerViewModel : PlayerViewModel
{
    public SamplePlayerViewModel() : base(new MockPlayerView(), null!, null!)
    {
        PlaylistManager = new PlaylistManager
        {
            CurrentItem = new PlaylistItem(new SampleMusicFile(new MusicMetadata(new TimeSpan(0, 3, 45), 320)
            {
                Artists = [ @"Culture Beat" ],
                Title = @"Serenity (Epilog)",
                Genre = [ "Electronic", "Dance" ]
            }, @"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity"))
        };
    }


    private class MockPlayerView : MockView, IPlayerView
    {
        public TimeSpan GetPosition() => new(0, 3, 33);

        public void SetPosition(TimeSpan position) { }
    }
}
