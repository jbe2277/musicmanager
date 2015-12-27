using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Waf.Foundation;

namespace Waf.MusicManager.Domain.MusicFiles
{
    public abstract class Entity : Model
    {
        private readonly HashSet<string> changes;
        private bool hasChanges;


        protected Entity()
        {
            this.changes = new HashSet<string>();
        }


        public bool HasChanges 
        {
            get { return hasChanges; }
            private set { SetProperty(ref hasChanges, value); }
        }


        public IReadOnlyCollection<string> GetChanges()
        { 
            return changes.ToArray(); 
        }

        public void ClearChanges()
        {
            HasChanges = false;
            changes.Clear();
        }

        protected bool SetPropertyAndTrackChanges<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (SetProperty(ref field, value, propertyName))
            {
                HasChanges = true;
                changes.Add(propertyName);
                return true;
            }
            return false;
        }
    }
}
