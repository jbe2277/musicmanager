using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Domain.Playlists;
using Waf.MusicManager.Presentation.Converters;

namespace Test.MusicManager.Presentation.Converters;

[TestClass]
public class IsPlaylistItemPlayingMultiConverterTest
{
    [TestMethod]
    public void ConvertTest()
    {
        var item1 = new PlaylistItem(null!);
        var item2 = new PlaylistItem(null!);
            
        var converter = new IsPlaylistItemPlayingMultiConverter();
        Assert.IsTrue((bool)converter.Convert([ item1, item1 ], null, null, null));
        Assert.IsFalse((bool)converter.Convert([ item1, item2 ], null, null, null));
    }
}
