using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Waf.UnitTesting;
using Waf.MusicManager.Presentation.Converters;
using Waf.MusicManager.Presentation.Properties;

namespace Test.MusicManager.Presentation.Converters;

[TestClass]
public class TotalDurationConverterTest
{
    [TestMethod]
    public void ConvertTest()
    {
        var converter = new TotalDurationConverter();
        Assert.AreEqual(string.Format(CultureInfo.CurrentCulture, Resources.AboutDuration, "0:03"), converter.Convert([ true, TimeSpan.FromSeconds(3) ], null, null, null));
        Assert.AreEqual("0:04", converter.Convert([ false, TimeSpan.FromSeconds(4) ], null, null, null));

        AssertHelper.ExpectedException<NotSupportedException>(() => converter.ConvertBack(null, null, null, null));
    }
}
