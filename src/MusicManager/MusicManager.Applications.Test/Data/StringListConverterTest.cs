using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Waf.UnitTesting;
using Waf.MusicManager.Applications.Data;

namespace Test.MusicManager.Applications.Data;

[TestClass]
public class StringListConverterTest
{
    [TestMethod]
    public void ToStringTest()
    {
        var nl = Environment.NewLine;
        var ls = CultureInfo.CurrentCulture.TextInfo.ListSeparator;

        Assert.AreEqual("", StringListConverter.ToString(null));
        Assert.AreEqual("", StringListConverter.ToString(Array.Empty<string>()));
        Assert.AreEqual("", StringListConverter.ToString(new[] { "" }));

        Assert.AreEqual("Pop", StringListConverter.ToString(new[] { "Pop" }));
        Assert.AreEqual("Pop" + ls + " Rock", StringListConverter.ToString(new[] { "Pop", "Rock" }));
        Assert.AreEqual("Pop" + nl + "Rock", StringListConverter.ToString(new[] { "Pop", "Rock" }, nl));
    }

    [TestMethod]
    public void ConvertBackTest()
    {
        var nl = Environment.NewLine;
        var ls = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            
        AssertHelper.SequenceEqual(Array.Empty<string>(), StringListConverter.FromString(""));

        AssertHelper.SequenceEqual(new[] { "Pop" }, StringListConverter.FromString("Pop"));
        AssertHelper.SequenceEqual(new[] { "Pop", "Rock" }, StringListConverter.FromString("Pop" + nl + "Rock", nl));
        AssertHelper.SequenceEqual(new[] { "Pop", "Rock" }, StringListConverter.FromString(" Pop" + nl + " Rock", nl));
        AssertHelper.SequenceEqual(new[] { "Pop", "Rock" }, StringListConverter.FromString("Pop " + nl + "Rock ", nl));

        AssertHelper.SequenceEqual(new[] { "Pop", "Rock" }, StringListConverter.FromString("Pop" + ls + "Rock"));
        AssertHelper.SequenceEqual(new[] { "Pop", "Rock" }, StringListConverter.FromString(" Pop" + ls + " Rock"));
        AssertHelper.SequenceEqual(new[] { "Pop", "Rock" }, StringListConverter.FromString("Pop " + ls + "Rock "));

        AssertHelper.SequenceEqual(new[] { "Pop" }, StringListConverter.FromString("Pop" + ls));
        AssertHelper.SequenceEqual(new[] { "Pop" }, StringListConverter.FromString("Pop" + ls + " "));
    }
}
