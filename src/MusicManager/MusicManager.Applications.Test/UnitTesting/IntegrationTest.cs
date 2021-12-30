using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.MusicManager.Applications.UnitTesting;

[TestClass]
public class IntegrationTest : ApplicationsTest
{
    protected override UnitTestLevel UnitTestLevel => UnitTestLevel.IntegrationTest;
}
