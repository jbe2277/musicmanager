using System.Collections.Concurrent;

namespace Waf.MusicManager.Domain.MusicFiles
{
    public static class ServiceLocator
    {
        private static readonly ConcurrentDictionary<Type, object> services = new();

        public static TId Get<TId>() where TId : class => (TId)services[typeof(TId)];

        public static void RegisterInstance<TId>(TId instance) where TId : class => services[typeof(TId)] = instance;
    }
}
