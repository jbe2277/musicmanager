using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waf.MusicManager.Domain.Playlists;
using Test.MusicManager.Domain.UnitTesting;

namespace Test.MusicManager.Domain.Playlists
{
    [TestClass]
    public class PlayedItemsStackTest : DomainTest
    {
        [TestMethod]
        public void StackTest()
        {
            var items = new[] { new object(), new object(), new object(), new object() };
            
            var stack = new PlayedItemsStack<object>(3);
            Assert.AreEqual(0, stack.Count);
            stack.Add(items[0]);
            Assert.AreEqual(items[0], stack.Pop());
            Assert.AreEqual(0, stack.Count);

            stack.Add(items[0]);
            stack.Add(items[1]);
            stack.Add(items[2]);
            Assert.AreEqual(3, stack.Count);
            Assert.IsTrue(stack.Contains(items[0]));
            Assert.IsFalse(stack.Contains(items[3]));

            stack.Add(items[3]);
            Assert.AreEqual(3, stack.Count);
            Assert.IsFalse(stack.Contains(items[0]));
            Assert.IsTrue(stack.Contains(items[3]));

            Assert.AreEqual(items[3], stack.Pop());
            Assert.AreEqual(2, stack.Count);

            stack.Add(items[2]);
            Assert.AreEqual(3, stack.Count);
            stack.RemoveAll(items[2]);
            Assert.AreEqual(1, stack.Count);

            stack.Clear();
            Assert.AreEqual(0, stack.Count);
        }
    }
}
