using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Domain.Playlists;

namespace Test.MusicManager.Domain.Playlists;

[TestClass]
public class StatisticsHelperTest
{
    [TestMethod]
    public void TruncatedMeanTest()
    {
        Assert.AreEqual(0, StatisticsHelper.TruncatedMean([], 0.25));

        Assert.AreEqual(6.5, StatisticsHelper.TruncatedMean([5, 8, 4, 38, 8, 6, 9, 7, 7, 3, 1, 6], 0.25));
    }
}
