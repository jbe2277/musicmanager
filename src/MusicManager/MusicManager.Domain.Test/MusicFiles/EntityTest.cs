using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Waf.UnitTesting;
using Waf.MusicManager.Domain.MusicFiles;
using Test.MusicManager.Domain.UnitTesting;

namespace Test.MusicManager.Domain.MusicFiles
{
    [TestClass]
    public class EntityTest : DomainTest
    {
        protected override void OnInitialize()
        {
            base.OnInitialize();
            ServiceLocator.RegisterInstance<IChangeTrackerService>(new MockChangeTrackerService());
        }


        [TestMethod]
        public void ChangeTrackerTest()
        {
            var entity = new MockEntity();
            entity.EntityLoadCompleted();
            Assert.IsFalse(entity.HasChanges);
            var changesSnapshot1 = entity.GetChanges();
            Assert.IsFalse(changesSnapshot1.Any());

            AssertHelper.PropertyChangedEvent(entity, x => x.HasChanges, () => entity.Name = "Bill");
            Assert.IsTrue(entity.HasChanges);
            var changesSnapshot2 = entity.GetChanges();
            Assert.IsFalse(changesSnapshot1.Any());
            Assert.AreEqual("Name", changesSnapshot2.Single());

            AssertHelper.PropertyChangedEvent(entity, x => x.HasChanges, () => entity.ClearChanges());
            Assert.IsFalse(entity.HasChanges);
            var changesSnapshot3 = entity.GetChanges();
            Assert.IsFalse(changesSnapshot1.Any());
            Assert.AreEqual("Name", changesSnapshot2.Single());
            Assert.IsFalse(changesSnapshot3.Any());

            entity.Name = "Bill";
            Assert.IsFalse(entity.HasChanges);
        }


        private class MockEntity : Entity
        {
            private string name;

            public string Name
            {
                get { return name; }
                set { SetPropertyAndTrackChanges(ref name, value); }
            }
        }
    }
}
