using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using Waf.MusicManager.Presentation;

namespace Test.MusicManager.Presentation
{
    [TestClass]
    public class ListSortComparerTest
    {
        [TestMethod]
        public void CompareTest()
        {
            var comparer = new ListSortComparer<string>(string.CompareOrdinal, ListSortDirection.Ascending);
            Assert.IsTrue(comparer.Compare("a", "z") < 0);
            Assert.AreEqual(0, comparer.Compare("a", "a"));
            Assert.IsTrue(comparer.Compare("b", "a") > 0);

            comparer = new ListSortComparer<string>(string.CompareOrdinal, ListSortDirection.Descending);
            Assert.IsTrue(comparer.Compare("a", "z") > 0);
            Assert.AreEqual(0, comparer.Compare("a", "a"));
            Assert.IsTrue(comparer.Compare("b", "a") < 0);
        }
    }
}
