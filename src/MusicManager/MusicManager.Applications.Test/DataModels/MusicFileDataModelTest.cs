using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Waf.UnitTesting;
using Test.MusicManager.Applications.UnitTesting;
using Test.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Domain.MusicFiles;

namespace Test.MusicManager.Applications.DataModels;

[TestClass]
public class MusicFileDataModelTest : ApplicationsTest
{
    [TestMethod]
    public void RaisePropertyChangedTest()
    {
        var ls = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            
        var musicFile = new MockMusicFile(new MusicMetadata(new TimeSpan(0, 3, 33), 320)
        {
            Artists = Array.Empty<string>(),
            Title = ""
        }, @"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity.mp3");
        var dataModel = new MusicFileDataModel(musicFile);

        Assert.AreEqual("", dataModel.ArtistsString);

        AssertHelper.PropertyChangedEvent(dataModel, x => x.ArtistsString, () => musicFile.Metadata!.Artists = new[] { "Culture Beat" });
        Assert.AreEqual("Culture Beat", dataModel.ArtistsString);
  
        AssertHelper.PropertyChangedEvent(dataModel, x => x.ArtistsString, () => musicFile.Metadata!.Artists = new[] { "Culture Beat", "Second artist" });
        Assert.AreEqual("Culture Beat" + ls + " Second artist", dataModel.ArtistsString);
    }
}
