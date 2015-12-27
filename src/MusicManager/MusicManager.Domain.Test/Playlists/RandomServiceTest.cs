using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Waf.UnitTesting;
using Waf.MusicManager.Domain.Playlists;
using Test.MusicManager.Domain.UnitTesting;

namespace Test.MusicManager.Domain.MusicFiles
{
    [TestClass]
    public class RandomServiceTest : DomainTest
    {
        [TestMethod]
        public void NextRandomNumber()
        {
            var randomService = new RandomService();

            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(randomService.NextRandomNumber(2) <= 2);
            }

            AssertHelper.ExpectedException<ArgumentOutOfRangeException>(() => randomService.NextRandomNumber(int.MaxValue));   
        }
    }
}
