using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Waf.MusicManager.Domain.MusicFiles;
using Test.MusicManager.Domain.UnitTesting;

namespace Test.MusicManager.Domain.MusicFiles
{
    [TestClass]
    public class GenresTest : DomainTest
    {
        [TestMethod]
        public void DefaultValuesTest()
        {
            Assert.IsTrue(Genres.DefaultValues.Count() > 5);
        }
    }
}
