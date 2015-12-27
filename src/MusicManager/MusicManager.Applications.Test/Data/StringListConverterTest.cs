using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Linq;
using Waf.MusicManager.Applications.Data;

namespace Test.MusicManager.Applications.Data
{
    [TestClass]
    public class StringListConverterTest
    {
        [TestMethod]
        public void ToStringTest()
        {
            var nl = Environment.NewLine;
            var ls = CultureInfo.CurrentCulture.TextInfo.ListSeparator;

            Assert.AreEqual("", StringListConverter.ToString(null));
            Assert.AreEqual("", StringListConverter.ToString(new string[0]));
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
            
            Assert.IsTrue((new string[0]).SequenceEqual(StringListConverter.FromString("")));

            Assert.IsTrue((new[] { "Pop" }).SequenceEqual(StringListConverter.FromString("Pop")));
            Assert.IsTrue((new[] { "Pop", "Rock" }).SequenceEqual(StringListConverter.FromString("Pop" + nl + "Rock", nl)));
            Assert.IsTrue((new[] { "Pop", "Rock" }).SequenceEqual(StringListConverter.FromString(" Pop" + nl + " Rock", nl)));
            Assert.IsTrue((new[] { "Pop", "Rock" }).SequenceEqual(StringListConverter.FromString("Pop " + nl + "Rock ", nl)));

            Assert.IsTrue((new[] { "Pop", "Rock" }).SequenceEqual(StringListConverter.FromString("Pop" + ls + "Rock")));
            Assert.IsTrue((new[] { "Pop", "Rock" }).SequenceEqual(StringListConverter.FromString(" Pop" + ls + " Rock")));
            Assert.IsTrue((new[] { "Pop", "Rock" }).SequenceEqual(StringListConverter.FromString("Pop " + ls + "Rock ")));

            Assert.IsTrue((new[] { "Pop" }).SequenceEqual(StringListConverter.FromString("Pop" + ls)));
            Assert.IsTrue((new[] { "Pop" }).SequenceEqual(StringListConverter.FromString("Pop" + ls + " ")));
        }
    }
}
