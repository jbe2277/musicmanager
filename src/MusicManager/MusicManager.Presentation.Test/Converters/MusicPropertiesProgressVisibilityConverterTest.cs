using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.UnitTesting;
using System.Windows;
using Waf.MusicManager.Presentation.Converters;

namespace Test.MusicManager.Presentation.Converters;

[TestClass]
public class MusicPropertiesProgressVisibilityConverterTest
{
    [TestMethod]
    public void ConvertTest()
    {
        var converter = new MusicPropertiesProgressVisibilityConverter();
        Assert.AreEqual(Visibility.Visible, converter.Convert([ false, null ], null, null, null));
        Assert.AreEqual(Visibility.Collapsed, converter.Convert([ true, null ], null, null, null));
        Assert.AreEqual(Visibility.Collapsed, converter.Convert([ false, new ArgumentException() ], null, null, null));

        AssertHelper.ExpectedException<NotSupportedException>(() => converter.ConvertBack(null, null, null, null));
    }
}
