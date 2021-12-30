using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Applications.Services;

namespace Test.MusicManager.Applications.Services;

[TestClass]
public class MusicTitleHelperTest
{
    [TestMethod]
    public void GetTitleTextTest()
    {
        var fileName = @"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity.mp3";

        Assert.AreEqual("Culture Beat - Serenity", MusicTitleHelper.GetTitleText(fileName, Array.Empty<string>(), null));
        Assert.AreEqual("Culture Beat - Serenity", MusicTitleHelper.GetTitleText(fileName, Array.Empty<string>(), ""));

        Assert.AreEqual("", MusicTitleHelper.GetTitleText(fileName, new[] { "Culture Beat" }, null));
        Assert.AreEqual("", MusicTitleHelper.GetTitleText(fileName, new[] { "Culture Beat" }, ""));

        Assert.AreEqual("Serenity (Epilog)", MusicTitleHelper.GetTitleText(fileName, new[] { "Culture Beat" }, "Serenity (Epilog)"));
    }
}
