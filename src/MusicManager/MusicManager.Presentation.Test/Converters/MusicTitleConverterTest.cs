using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Waf.UnitTesting;
using Waf.MusicManager.Presentation.Converters;

namespace Test.MusicManager.Presentation.Converters
{
    [TestClass]
    public class MusicTitleConverterTest
    {
        [TestMethod]
        public void ConvertTest()
        {
            var converter = new MusicTitleConverter();
            var fileName = @"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity.mp3";

            Assert.AreEqual("Culture Beat - Serenity", ConvertCore(converter, fileName, new string[0], null));
            Assert.AreEqual("Culture Beat - Serenity", ConvertCore(converter, fileName, new string[0], ""));

            Assert.AreEqual("", ConvertCore(converter, fileName, new[] { "Culture Beat" }, null));
            Assert.AreEqual("", ConvertCore(converter, fileName, new[] { "Culture Beat" }, ""));

            Assert.AreEqual("Serenity (Epilog)", ConvertCore(converter, fileName, new[] { "Culture Beat" }, "Serenity (Epilog)"));

            AssertHelper.ExpectedException<NotSupportedException>(() => converter.ConvertBack(null, null, null, null));
        }

        private string ConvertCore(MusicTitleConverter converter, string fileName, IEnumerable<string> artists, string title)
        {
            return (string)converter.Convert(new object[] { fileName, artists, title }, null, null, null);
        }
    }
}
