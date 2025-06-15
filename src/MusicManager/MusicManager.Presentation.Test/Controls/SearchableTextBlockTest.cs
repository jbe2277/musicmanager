using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.UnitTesting;
using Waf.MusicManager.Presentation.Controls;

namespace Test.MusicManager.Presentation.Controls;

[TestClass]
public class SearchableTextBlockTest
{
    [TestMethod]
    public void SplitTextTest()
    {
        AssertHelper.SequenceEqual([""], SearchableTextBlock.SplitText("", "", false));
        AssertHelper.SequenceEqual([""], SearchableTextBlock.SplitText("", "", true));

        AssertHelper.SequenceEqual(["Hello"], SearchableTextBlock.SplitText("Hello", "", false));
        AssertHelper.SequenceEqual(["", "Hello", ""], SearchableTextBlock.SplitText("Hello", "Hello", false));
        AssertHelper.SequenceEqual(["", "Hello", " World"], SearchableTextBlock.SplitText("Hello World", "Hello", false));
        AssertHelper.SequenceEqual(["", "Hello", "", "Hello", "-", "Hello", "."], SearchableTextBlock.SplitText("HelloHello-Hello.", "Hello", false));

        AssertHelper.SequenceEqual(["", "Hello", ""], SearchableTextBlock.SplitText("Hello", "Hello", true));
        AssertHelper.SequenceEqual(["Hello"], SearchableTextBlock.SplitText("Hello", "HELLO", true));
        AssertHelper.SequenceEqual(["Hello", "HELLO", ""], SearchableTextBlock.SplitText("HelloHELLO", "HELLO", true));
    }
}
