using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Internal.Messaging
{
   
    public class Broker : IBroker
    {

        private readonly ConcurrentDictionary<Type,  List<DescribedDelegate>> _subscribers;

        public Broker()
        {
            _subscribers = new ConcurrentDictionary<Type, List<DescribedDelegate>>();
        }

        public Task<IEnumerable<Func<Message<T>, Task<T>>>> Pub<T>(object sender, MessageOptions options, T message) where T : class
        {

            var ourput = new List<Func<Message<T>, Task<T>>>();
            
            if (message == null || sender == null)
            {
                return Task.FromResult((IEnumerable<Func<Message<T>, Task<T>>>)ourput);
            }

            if (!_subscribers.ContainsKey(typeof(T)))
            {
                return Task.FromResult((IEnumerable<Func<Message<T>, Task<T>>>)ourput);

            }

            var delegates = _subscribers[typeof(T)];
            if (delegates == null || delegates.Count == 0)
            {
                return Task.FromResult((IEnumerable<Func<Message<T>, Task<T>>>)ourput);

            }

            var payload = new Message<T>(message, sender);

            // Iterate through action delegates and invoke
            foreach (var handler in delegates
                .Where(d => d.Options.Key == options.Key)
                .Select(s => s.Subscription as Action<Message<T>>))
            {
                if (handler != null)
                {
                    // wrap our action delegate within a func with a false return type to allow for deferred exceution
                    ourput.Add(new Func<Message<T>, Task<T>>((Message<T> input) =>
                    {
                        handler.Invoke(payload);
                        return Task.FromResult((T) input.What);
                    }));
                    handler.Invoke(payload);
                }         
              
            }

            // Iterate through func delegates and return 
            foreach (var handler in delegates
                .Where(d => d.Options.Key == options.Key)
                .Select(s => s.Subscription as Func<Message<T>, Task<T>>))
            {
                if (handler != null)
                {
                    // wrapper to convert delegates generic argument type
                    // to concrete type (object) to allow for deferred execurtion
                    ourput.Add(new Func<Message<T>, Task<T>>((Message<T> input) => handler(input)));
                }
            }

            return Task.FromResult((IEnumerable<Func<Message<T>, Task<T>>>) ourput);

        }

        public void Sub<T>(MessageOptions options, Action<Message<T>> subscription) where T : class
        {
            var describedDelegate = new DescribedDelegate(options, subscription);

            var delegates = _subscribers.ContainsKey(typeof(T))
                ? _subscribers[typeof(T)]
                : new List<DescribedDelegate>();

            if (!delegates.Contains(describedDelegate))
            {
                delegates.Add(describedDelegate);
            }
            _subscribers[typeof(T)] = delegates;
        }
        
        public void Sub<T>(MessageOptions options, Func<Message<T>, Task<T>> subscription) where T : class
        {
            var describedDelegate = new DescribedDelegate(options, subscription);

            var delegates = _subscribers.ContainsKey(typeof(T)) ?
                _subscribers[typeof(T)] : new List<DescribedDelegate>();

            if (!delegates.Contains(describedDelegate))
            {
                delegates.Add(describedDelegate);
            }
            _subscribers[typeof(T)] = delegates;
        }

        public void Unsub<T>(MessageOptions options, Action<Message<T>> subscription) where T : class
        {
            var describedDelegate = new DescribedDelegate(options, subscription);

            if (!_subscribers.ContainsKey(typeof(T))) return;
            var delegates = _subscribers[typeof(T)];
            if (delegates.Contains(describedDelegate))
            {
                delegates.Remove(describedDelegate);
            }
            if (delegates.Count == 0)
            {
                _subscribers.TryRemove(typeof(T), out List<DescribedDelegate> method);
            }
        }

        public void Unsub<T>(MessageOptions options, Func<Message<T>, Task<T>> subscription) where T : class
        {

            var describedDelegate = new DescribedDelegate(options, subscription);

            if (!_subscribers.ContainsKey(typeof(T))) return;
            var delegates = _subscribers[typeof(T)];
            if (delegates.Contains(describedDelegate))
            {
                delegates.Remove(describedDelegate);
            }
            if (delegates.Count == 0)
            {
                _subscribers.TryRemove(typeof(T), out List<DescribedDelegate> method);
            }
                
        }
        
        public void Dispose()
        {
            _subscribers?.Clear();
        }


    }

}
