using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Waf.UnitTesting;
using Waf.MusicManager.Presentation.Converters;

namespace Test.MusicManager.Presentation.Converters
{
    [TestClass]
    public class StringListToStringConverterTest
    {
        [TestMethod]
        public void ConvertTest()
        {
            var nl = Environment.NewLine;
            var ls = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            var converter = new StringListToStringConverter();

            Assert.AreEqual("", converter.Convert(null, null, null, null));
            Assert.AreEqual("", converter.Convert(Array.Empty<string>(), null, null, null));
            Assert.AreEqual("", converter.Convert(new[] { "" }, null, null, null));

            Assert.AreEqual("Pop", converter.Convert(new[] { "Pop" }, null, null, null));
            Assert.AreEqual("Pop" + nl + "Rock", converter.Convert(new[] { "Pop", "Rock" }, null, null, null));
            Assert.AreEqual("Pop" + ls + " Rock", converter.Convert(new[] { "Pop", "Rock" }, null, "ListSeparator", null));
        }

        [TestMethod]
        public void ConvertBackTest()
        {
            var nl = Environment.NewLine;
            var ls = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            var converter = new StringListToStringConverter();

            AssertHelper.SequenceEqual(Array.Empty<string>(), (string[])converter.ConvertBack("", null, null, null));

            AssertHelper.SequenceEqual(new[] { "Pop" }, (string[])converter.ConvertBack("Pop", null, null, null));
            AssertHelper.SequenceEqual(new[] { "Pop", "Rock" }, (string[])converter.ConvertBack("Pop" + nl + "Rock", null, null, null));
            AssertHelper.SequenceEqual(new[] { "Pop", "Rock" }, (string[])converter.ConvertBack(" Pop" + nl + " Rock", null, null, null));
            AssertHelper.SequenceEqual(new[] { "Pop", "Rock" }, (string[])converter.ConvertBack("Pop " + nl + "Rock ", null, null, null));

            AssertHelper.SequenceEqual(new[] { "Pop", "Rock" }, (string[])converter.ConvertBack("Pop" + ls + "Rock", null, "ListSeparator", null));
            AssertHelper.SequenceEqual(new[] { "Pop", "Rock" }, (string[])converter.ConvertBack(" Pop" + ls + " Rock", null, "ListSeparator", null));
            AssertHelper.SequenceEqual(new[] { "Pop", "Rock" }, (string[])converter.ConvertBack("Pop " + ls + "Rock ", null, "ListSeparator", null));

            AssertHelper.SequenceEqual(new[] { "Pop" }, (string[])converter.ConvertBack("Pop" + ls, null, "ListSeparator", null));
            AssertHelper.SequenceEqual(new[] { "Pop" }, (string[])converter.ConvertBack("Pop" + ls + " ", null, "ListSeparator", null));
        }
    }
}
