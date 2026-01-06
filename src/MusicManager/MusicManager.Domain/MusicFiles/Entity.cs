using System.Runtime.CompilerServices;
using System.Waf.Foundation;

namespace Waf.MusicManager.Domain.MusicFiles;

public abstract class Entity : Model
{
    private readonly Lazy<IChangeTrackerService> changeTrackerService;
    private readonly HashSet<string> changes = [];
    private bool entityLoaded;

    protected Entity()
    {
        changeTrackerService = new(ServiceLocator.Get<IChangeTrackerService>);
    }

    public bool HasChanges { get; private set => SetProperty(ref field, value); }

    public IReadOnlySet<string> Changes => changes;

    public void EntityLoadCompleted() => entityLoaded = true;

    public void ClearChanges()
    {
        changes.Clear();
        HasChanges = false;
    }

    protected bool SetPropertyAndTrackChanges<T>([NotNullIfNotNull(parameterName: nameof(value)), MaybeNull] ref T field, [AllowNull] T value, [CallerMemberName] string propertyName = null!)
    {
        if (!SetProperty(ref field, value, propertyName)) return false;
        if (entityLoaded)
        {
            changes.Add(propertyName);
            HasChanges = true;
        }
        return true;
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
