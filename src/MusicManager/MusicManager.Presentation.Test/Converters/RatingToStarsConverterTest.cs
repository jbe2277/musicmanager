using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Presentation.Converters;

namespace Test.MusicManager.Presentation.Converters
{
    [TestClass]
    public class RatingToStarsConverterTest
    {
        [TestMethod]
        public void ConvertTest()
        {
            var converter = new RatingToStarsConverter();
            Assert.AreEqual(5, converter.Convert(100, null, null, null));
            Assert.AreEqual(5, converter.Convert(99, null, null, null));

            Assert.AreEqual(4, converter.Convert(98, null, null, null));
            Assert.AreEqual(4, converter.Convert(75, null, null, null));

            Assert.AreEqual(3, converter.Convert(74, null, null, null));
            Assert.AreEqual(3, converter.Convert(50, null, null, null));

            Assert.AreEqual(2, converter.Convert(49, null, null, null));
            Assert.AreEqual(2, converter.Convert(25, null, null, null));

            Assert.AreEqual(1, converter.Convert(24, null, null, null));
            Assert.AreEqual(1, converter.Convert(1, null, null, null));

            Assert.AreEqual(0, converter.Convert(0, null, null, null));
            Assert.AreEqual(0, converter.Convert(-1, null, null, null));
        }

        [TestMethod]
        public void ConvertBackTest()
        {
            var converter = new RatingToStarsConverter();
            Assert.AreEqual(99, converter.ConvertBack(5, null, null, null));
            Assert.AreEqual(75, converter.ConvertBack(4, null, null, null));
            Assert.AreEqual(50, converter.ConvertBack(3, null, null, null));
            Assert.AreEqual(25, converter.ConvertBack(2, null, null, null));
            Assert.AreEqual(1, converter.ConvertBack(1, null, null, null));
            Assert.AreEqual(0, converter.ConvertBack(0, null, null, null));
            Assert.AreEqual(0, converter.ConvertBack(-1, null, null, null));
        }
    }
}
