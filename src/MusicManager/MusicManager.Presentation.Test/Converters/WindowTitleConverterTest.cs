using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.UnitTesting;
using Waf.MusicManager.Presentation.Converters;

namespace Test.MusicManager.Presentation.Converters;

[TestClass]
public class WindowTitleConverterTest
{
    [TestMethod]
    public void ConvertTest()
    {
        var converter = new WindowTitleConverter();
            
        Assert.AreEqual("Waf Music Manager", converter.Convert([ null, null, "Waf Music Manager" ], null, null, null));
            
        Assert.AreEqual("Culture Beat - Waf Music Manager", converter.Convert([ "Culture Beat", null, "Waf Music Manager" ], null, null, null));
            
        Assert.AreEqual("Culture Beat - Serenity (Epilog) - Waf Music Manager", converter.Convert([ "Culture Beat", "Serenity (Epilog)", "Waf Music Manager" ], null, null, null));

        AssertHelper.ExpectedException<NotSupportedException>(() => converter.ConvertBack(null, null, null, null));
    }
}
