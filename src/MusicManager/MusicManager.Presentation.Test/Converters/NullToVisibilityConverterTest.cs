using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using Waf.MusicManager.Presentation.Converters;

namespace Test.MusicManager.Presentation.Converters
{
    [TestClass]
    public class NullToVisibilityConverterTest
    {
        [TestMethod]
        public void ConvertTest()
        {
            var converter = new NullToVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert("Test", null, null, null));
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(null, null, null, null));

            Assert.AreEqual(Visibility.Collapsed, converter.Convert("Test", null, "invert", null));
            Assert.AreEqual(Visibility.Visible, converter.Convert(null, null, "Invert", null));
        }
    }
}
