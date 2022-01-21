using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Domain.MusicFiles;

namespace Test.MusicManager.Domain.MusicFiles;

[TestClass]
public class GenresTest : DomainTest
{
    [TestMethod]
    public void DefaultValuesTest()
    {
        Assert.IsTrue(Genres.DefaultValues.Count > 5);
    }
}
