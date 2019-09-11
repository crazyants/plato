using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Plato.Internal.Abstractions
{

    public interface ISingletonCache<T> where T : class
    {

        bool ContainsKey(string key);

        T this[string key] { get; set; }
        
    }

    public class SingletonCache<T> : ISingletonCache<T> where T : class
    {
        
        private readonly ConcurrentDictionary<string, T> _cache 
            = new ConcurrentDictionary<string, T>();

        public T this[string key]
        {
            get => _cache.ContainsKey(key) ? _cache[key] : null;
            set => _cache[key] = value;
        }
        
        public bool ContainsKey(string key)
        {
            return _cache.ContainsKey(key);
        }
        
    }

}
