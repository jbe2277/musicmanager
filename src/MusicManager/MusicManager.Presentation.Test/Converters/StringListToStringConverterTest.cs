using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Linq;
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
            Assert.AreEqual("", converter.Convert(new string[0], null, null, null));
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

            Assert.IsTrue((new string[0]).SequenceEqual((string[])converter.ConvertBack("", null, null, null)));
            
            Assert.IsTrue((new[] { "Pop" }).SequenceEqual((string[])converter.ConvertBack("Pop", null, null, null)));
            Assert.IsTrue((new[] { "Pop", "Rock" }).SequenceEqual((string[])converter.ConvertBack("Pop" + nl + "Rock", null, null, null)));
            Assert.IsTrue((new[] { "Pop", "Rock" }).SequenceEqual((string[])converter.ConvertBack(" Pop" + nl + " Rock", null, null, null)));
            Assert.IsTrue((new[] { "Pop", "Rock" }).SequenceEqual((string[])converter.ConvertBack("Pop " + nl + "Rock ", null, null, null)));
            
            Assert.IsTrue((new[] { "Pop", "Rock" }).SequenceEqual((string[])converter.ConvertBack("Pop" + ls + "Rock", null, "ListSeparator", null)));
            Assert.IsTrue((new[] { "Pop", "Rock" }).SequenceEqual((string[])converter.ConvertBack(" Pop" + ls + " Rock", null, "ListSeparator", null)));
            Assert.IsTrue((new[] { "Pop", "Rock" }).SequenceEqual((string[])converter.ConvertBack("Pop " + ls + "Rock ", null, "ListSeparator", null)));
            
            Assert.IsTrue((new[] { "Pop" }).SequenceEqual((string[])converter.ConvertBack("Pop" + ls, null, "ListSeparator", null)));
            Assert.IsTrue((new[] { "Pop" }).SequenceEqual((string[])converter.ConvertBack("Pop" + ls + " ", null, "ListSeparator", null)));
        }
    }
}
