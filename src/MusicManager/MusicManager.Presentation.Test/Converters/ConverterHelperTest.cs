using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Presentation.Converters;

namespace Test.MusicManager.Presentation.Converters;

[TestClass]
public class ConverterHelperTest
{
    [TestMethod]
    public void IsParameterSetTest()
    {
        Assert.IsFalse(ConverterHelper.IsParameterSet("test", null));
        Assert.IsFalse(ConverterHelper.IsParameterSet("test", new object()));
        Assert.IsFalse(ConverterHelper.IsParameterSet("test", ""));
        Assert.IsFalse(ConverterHelper.IsParameterSet("test", "tes"));

        Assert.IsTrue(ConverterHelper.IsParameterSet("test", "test"));
        Assert.IsTrue(ConverterHelper.IsParameterSet("test", "TEST"));
        Assert.IsTrue(ConverterHelper.IsParameterSet("TEST", "test"));
    }
}
