using System;
using Waf.MusicManager.Domain.MusicFiles;

namespace Test.MusicManager.Domain.MusicFiles
{
    internal class MockChangeTrackerService : IChangeTrackerService
    {
        public Action<Entity> EntityHasChangesAction { get; set; }

        public void EntityHasChanges(Entity entity)
        {
            EntityHasChangesAction?.Invoke(entity);
        }
    }
}
