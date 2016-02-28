using System.Collections.Generic;
using System.Linq;
using Waf.MusicManager.Domain.MusicFiles;

namespace Test.MusicManager.Domain.MusicFiles
{
    internal class MockChangeTrackerService : IChangeTrackerService
    {
        private HashSet<Entity> entitiesWithChanges;


        public MockChangeTrackerService()
        {
            entitiesWithChanges = new HashSet<Entity>();
        }


        public IEnumerable<Entity> GetEntitiesWithChanges()
        {
            return entitiesWithChanges.ToArray();
        }

        public void EntityHasChanges(Entity entity)
        {
            entitiesWithChanges.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            entitiesWithChanges.Remove(entity);
        }
    }
}
