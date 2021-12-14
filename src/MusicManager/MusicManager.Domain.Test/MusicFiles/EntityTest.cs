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
        private MockChangeTrackerService changeTrackerService = null!;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            changeTrackerService = new MockChangeTrackerService();
            ServiceLocator.RegisterInstance<IChangeTrackerService>(changeTrackerService);
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

        [TestMethod]
        public void EntityHasChangesTest()
        {
            int entityHasChangesCallCount = 0;
            changeTrackerService.EntityHasChangesAction = e => entityHasChangesCallCount++;

            // No change tracking during loading state
            var entity = new MockEntity();
            entity.Name = "Bill";
            Assert.AreEqual(0, entityHasChangesCallCount);
            Assert.IsFalse(entity.HasChanges);

            // Start change tracking when load completed
            entity.EntityLoadCompleted();
            entity.Name = "Steve";
            Assert.AreEqual(1, entityHasChangesCallCount);
            Assert.IsTrue(entity.HasChanges);
        }


        private class MockEntity : Entity
        {
            private string? name;

            public string? Name
            {
                get => name;
                set => SetPropertyAndTrackChanges(ref name, value);
            }
        }
    }
}
