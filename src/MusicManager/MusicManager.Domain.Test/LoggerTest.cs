using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Domain;

namespace Test.MusicManager.Domain
{
    [TestClass]
    public class LoggerTest
    {
        [TestMethod]
        public void GetMemberNameTest()
        {
            Assert.AreEqual("GetMemberNameTest", Logger.GetMemberName());
        }
    }
}
