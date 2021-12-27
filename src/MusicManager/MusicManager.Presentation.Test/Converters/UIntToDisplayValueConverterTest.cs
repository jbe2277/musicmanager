using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Presentation.Converters;

namespace Test.MusicManager.Presentation.Converters
{
    [TestClass]
    public class UIntToDisplayValueConverterTest
    {
        [TestMethod]
        public void ConvertTest()
        {
            var converter = new UIntToDisplayValueConverter();
            Assert.AreEqual(3u, converter.Convert(3u, null, null, null));
            Assert.AreEqual("", converter.Convert(0u, null, null, null));
        }

        [TestMethod]
        public void ConvertBackTest()
        {
            var converter = new UIntToDisplayValueConverter();
            Assert.AreEqual(3u, converter.ConvertBack("3", null, null, null));
            Assert.AreEqual(0u, converter.ConvertBack("", null, null, null));
            Assert.AreEqual(0u, converter.ConvertBack(null, null, null, null));
        }
    }
}
