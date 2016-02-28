﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Waf.Foundation;

namespace Waf.MusicManager.Domain.MusicFiles
{
    public abstract class Entity : Model
    {
        private readonly HashSet<string> changes;
        private bool entityLoaded;
        private bool hasChanges;
        private Lazy<IChangeTrackerService> changeTrackerService;


        protected Entity()
        {
            changes = new HashSet<string>();
            changeTrackerService = new Lazy<IChangeTrackerService>(() => ServiceLocator.Get<IChangeTrackerService>());
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

        public void EntityLoadCompleted()
        {
            entityLoaded = true;
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
                if (entityLoaded)
                {
                    HasChanges = true;
                    changes.Add(propertyName);
                }
                return true;
            }
            return false;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (entityLoaded && e.PropertyName == nameof(HasChanges) && HasChanges)
            {
                changeTrackerService.Value.EntityHasChanges(this);
            }
        }
    }
}
