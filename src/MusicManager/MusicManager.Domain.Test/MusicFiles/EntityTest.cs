using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.UnitTesting;
using Waf.MusicManager.Domain.MusicFiles;

namespace Test.MusicManager.Domain.MusicFiles;

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
        Assert.IsFalse(entity.Changes.Any());

        AssertHelper.PropertyChangedEvent(entity, x => x.HasChanges, () => entity.Name = "Bill");
        Assert.IsTrue(entity.HasChanges);
        Assert.AreEqual("Name", entity.Changes.Single());

        AssertHelper.PropertyChangedEvent(entity, x => x.HasChanges, () => entity.ClearChanges());
        Assert.IsFalse(entity.HasChanges);
        Assert.IsFalse(entity.Changes.Any());

        entity.Name = "Bill";
        Assert.IsFalse(entity.HasChanges);
    }

    [TestMethod]
    public void EntityHasChangesTest()
    {
        int entityHasChangesCallCount = 0;
        changeTrackerService.EntityHasChangesAction = e => entityHasChangesCallCount++;

        // No change tracking during loading state
        var entity = new MockEntity { Name = "Bill" };
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
        public string? Name { get; set => SetPropertyAndTrackChanges(ref field, value); }
    }
}
