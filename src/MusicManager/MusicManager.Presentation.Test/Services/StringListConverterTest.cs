using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Waf.UnitTesting;
using Waf.MusicManager.Presentation.Services;

namespace Test.MusicManager.Presentation.Services;

[TestClass]
public class StringListConverterTest
{
    [TestMethod]
    public void ToStringTest()
    {
        var nl = Environment.NewLine;
        var ls = CultureInfo.CurrentCulture.TextInfo.ListSeparator;

        Assert.AreEqual("", StringListConverter.ToString(null));
        Assert.AreEqual("", StringListConverter.ToString([]));
        Assert.AreEqual("", StringListConverter.ToString([ "" ]));

        Assert.AreEqual("Pop", StringListConverter.ToString([ "Pop" ]));
        Assert.AreEqual("Pop" + ls + " Rock", StringListConverter.ToString([ "Pop", "Rock" ]));
        Assert.AreEqual("Pop" + nl + "Rock", StringListConverter.ToString([ "Pop", "Rock" ], nl));
    }

    [TestMethod]
    public void ConvertBackTest()
    {
        var nl = Environment.NewLine;
        var ls = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            
        AssertHelper.SequenceEqual([], StringListConverter.FromString(""));

        AssertHelper.SequenceEqual([ "Pop" ], StringListConverter.FromString("Pop"));
        AssertHelper.SequenceEqual([ "Pop", "Rock" ], StringListConverter.FromString("Pop" + nl + "Rock", nl));
        AssertHelper.SequenceEqual([ "Pop", "Rock" ], StringListConverter.FromString(" Pop" + nl + " Rock", nl));
        AssertHelper.SequenceEqual([ "Pop", "Rock" ], StringListConverter.FromString("Pop " + nl + "Rock ", nl));

        AssertHelper.SequenceEqual([ "Pop", "Rock" ], StringListConverter.FromString("Pop" + ls + "Rock"));
        AssertHelper.SequenceEqual([ "Pop", "Rock" ], StringListConverter.FromString(" Pop" + ls + " Rock"));
        AssertHelper.SequenceEqual([ "Pop", "Rock" ], StringListConverter.FromString("Pop " + ls + "Rock "));

        AssertHelper.SequenceEqual([ "Pop" ], StringListConverter.FromString("Pop" + ls));
        AssertHelper.SequenceEqual([ "Pop" ], StringListConverter.FromString("Pop" + ls + " "));
    }
}
