using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Internal.Messaging
{
    
    public class Broker : IBroker
    {

        private readonly IDictionary<Type, List<Delegate>> _subscribers;

        public Broker()
        {
            _subscribers = new Dictionary<Type, List<Delegate>>();
        }

        public async Task<IEnumerable<Func<Message<T>, Task<T>>>> Pub<T>(object source, T message) where T : class
        {

            if (message == null || source == null)
                return null;
            if (!_subscribers.ContainsKey(typeof(T)))
            {
                return null;
            }

            var delegates = _subscribers[typeof(T)];
            if (delegates == null || delegates.Count == 0)
            {
                return null;
            }

            var payload = new Message<T>(message, source);

            foreach (var handler in delegates.Select
                (item => item as Action<Message<T>>))
            {
                handler?.Invoke(payload);
            }

            var ourput = new List<Func<Message<T>, Task<T>>>();
            foreach (var handler in delegates.Select
                (item => item as Func<Message<T>, Task<T>>))
            {
                if (handler != null)
                {
                    // wrapper to convert delegates generic argument type
                    // to concrete type (object) to allow for deferred execurtion
                    var typedDelegate = new Func<Message<T>, Task<T>>((Message<T> input) => handler(input));
                    ourput.Add(typedDelegate);
                }
            }

            return ourput;

        }

        public void Sub<T>(Action<Message<T>> subscription) where T : class
        {
            var delegates = _subscribers.ContainsKey(typeof(T)) ?
                _subscribers[typeof(T)] : new List<Delegate>();
            if (!delegates.Contains(subscription))
            {
                delegates.Add(subscription);
            }
            _subscribers[typeof(T)] = delegates;
        }
        
        public void Sub<T>(Func<Message<T>, Task<T>> subscription) where T : class
        {
            var delegates = _subscribers.ContainsKey(typeof(T)) ?
                _subscribers[typeof(T)] : new List<Delegate>();
            if (!delegates.Contains(subscription))
            {
                delegates.Add(subscription);
            }
            _subscribers[typeof(T)] = delegates;
        }

        public void Unsub<T>(Action<Message<T>> subscription) where T : class
        {
            if (!_subscribers.ContainsKey(typeof(T))) return;
            var delegates = _subscribers[typeof(T)];
            if (delegates.Contains(subscription))
                delegates.Remove(subscription);
            if (delegates.Count == 0)
                _subscribers.Remove(typeof(T));
        }

        public void Unsub<T>(Func<Message<T>, Task<T>> subscription) where T : class
        {
            if (!_subscribers.ContainsKey(typeof(T))) return;
            var delegates = _subscribers[typeof(T)];
            if (delegates.Contains(subscription))
                delegates.Remove(subscription);
            if (delegates.Count == 0)
                _subscribers.Remove(typeof(T));
        }


        public void Dispose()
        {
            _subscribers?.Clear();
        }


    }
}
