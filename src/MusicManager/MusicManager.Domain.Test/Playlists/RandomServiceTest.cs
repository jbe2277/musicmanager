using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.UnitTesting;
using Waf.MusicManager.Domain.Playlists;

namespace Test.MusicManager.Domain.Playlists;

[TestClass]
public class RandomServiceTest : DomainTest
{
    [TestMethod]
    public void NextRandomNumber()
    {
        var randomService = new RandomService();
        for (int i = 0; i < 10; i++) Assert.IsTrue(randomService.NextRandomNumber(2) <= 2);
            
        AssertHelper.ExpectedException<ArgumentOutOfRangeException>(() => randomService.NextRandomNumber(int.MaxValue));   
    }
}
