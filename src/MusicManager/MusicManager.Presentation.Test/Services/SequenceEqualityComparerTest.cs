using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Presentation.Services;

namespace Test.MusicManager.Presentation.Services;

[TestClass]
public class SequenceEqualityComparerTest
{
    [TestMethod]
    public void ComparerTest()
    {
        var list1 = new[] { "Luke", "Han", "Lea" };
        var list2 = new[] { "Luke", "Han", "Lea" };
        var list3 = new[] { "Luke", "Han", "Leo" };
            
        var comparer = SequenceEqualityComparer<string>.Default;
        Assert.IsTrue(comparer.Equals(list1, list2));
        Assert.IsFalse(comparer.Equals(list1, list3));
        Assert.IsTrue(comparer.Equals(null, null));
        Assert.IsFalse(comparer.Equals(list1, null));
        Assert.IsFalse(comparer.Equals(null, list1));

        Assert.AreEqual(list1![0].GetHashCode() ^ list1[1].GetHashCode() ^ list1[2].GetHashCode(), comparer.GetHashCode(list1));
        Assert.AreEqual("Han".GetHashCode(), comparer.GetHashCode(new[] { "Han" }));
        Assert.AreEqual(0, comparer.GetHashCode(System.Array.Empty<string>()));
    }
}
