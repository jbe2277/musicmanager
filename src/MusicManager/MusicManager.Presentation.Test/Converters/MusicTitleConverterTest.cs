using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.UnitTesting;
using Waf.MusicManager.Presentation.Converters;

namespace Test.MusicManager.Presentation.Converters;

[TestClass]
public class MusicTitleConverterTest
{
    [TestMethod]
    public void ConvertTest()
    {
        var converter = MusicTitleConverter.Default;
        var fileName = @"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity.mp3";

        Assert.AreEqual("Culture Beat - Serenity", ConvertCore(converter, fileName, [], null));
        Assert.AreEqual("Culture Beat - Serenity", ConvertCore(converter, fileName, [], ""));

        Assert.AreEqual("", ConvertCore(converter, fileName, [ "Culture Beat" ], null));
        Assert.AreEqual("", ConvertCore(converter, fileName, [ "Culture Beat" ], ""));

        Assert.AreEqual("Serenity (Epilog)", ConvertCore(converter, fileName, [ "Culture Beat" ], "Serenity (Epilog)"));

        AssertHelper.ExpectedException<NotSupportedException>(() => converter.ConvertBack(null, null, null, null));
    }

    private static string ConvertCore(MusicTitleConverter converter, string fileName, IEnumerable<string> artists, string? title)
    {
        return (string)converter.Convert([ fileName, artists, title ], null, null, null);
    }
}
