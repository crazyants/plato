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

        public IEnumerable<Func<Message<T>, Task<T>>> Pub<T>(object sender, MessageOptions options, T message) where T : class
        {

            var ourput = new List<Func<Message<T>, Task<T>>>();
            
            // Nothing to process return empty collection
            if (message == null || sender == null)
            {
                return ourput;
            }

            // No _subscribers for given type return empty collection
            if (!_subscribers.ContainsKey(typeof(T)))
            {
                return ourput;
            }

            // No delegates within subscriber for given type return empty collection
            var delegates = _subscribers[typeof(T)];
            if (delegates == null || delegates.Count == 0)
            {
                return ourput;
            }

            // The payload passwed to each subscriber delegate
            var delegatePayload = new Message<T>(message, sender);

            // Iterate through subscriber action delegates matching our key
            foreach (var handler in delegates
                .Where(d => d.Options.Key == options.Key)
                .Select(s => s.Subscription as Action<Message<T>>))
            {
                if (handler != null)
                {
                    // Action delegates return void and as such cannot be awaited
                    // Wrap action delegates within a dummy func delegate ensuring 
                    // the action can be executed consistently and asynchronously 
                    ourput.Add(new Func<Message<T>, Task<T>>(async (Message<T> input) =>
                    {
                        return await Task.Factory.StartNew(() =>
                        {
                            handler.Invoke(input);
                            return input.What;
                        });
                    }));
                }         
              
            }
            
            // Iterate through subscriber func delegates matching our key
            foreach (var func in delegates
                .Where(d => d.Options.Key == options.Key)
                .Select(s => s.Subscription as Func<Message<T>, Task<T>>))
            {
                if (func != null)
                {
                    // Wrap our subscriber delegate within a dummy delegate
                    // This allows us to invoke the dummy delegate externally
                    // passing in a custom message for our real subscriber delegate
                    ourput.Add(new Func<Message<T>, Task<T>>(async (Message<T> input) => await func.Invoke(input)));

                    // convert delegates generic type to
                    // concrete delegate type to allow for deferred invocation
                    //ourput.Add((Message<T> input) => func(delegatePayload));
                }
            }

            // Return funcs to invoke
            return ourput;

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

            var delegates = _subscribers.ContainsKey(typeof(T)) 
                ? _subscribers[typeof(T)] 
                : new List<DescribedDelegate>();

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

            // Get delegates for our type
            var typeDelegates = _subscribers[typeof(T)];

            // Get delegates for type matching our key
            var matchingKey = typeDelegates
                .Where(d => d.Options.Key == options.Key)
                .ToList();

            // Get delegates matching our subscription target
            var matchingDelegates = matchingKey
                .Where(d => d.Subscription.Target.Equals(subscription.Target))
                .ToList();

            // Remove
            foreach (var matchingDelegate in matchingDelegates)
            {
                if (typeDelegates.Contains(matchingDelegate))
                {
                    typeDelegates.Remove(matchingDelegate);
                }
            }
            
            if (typeDelegates.Count == 0)
            {
                _subscribers.TryRemove(typeof(T), out List<DescribedDelegate> method);
            }
        }

        public void Unsub<T>(MessageOptions options, Func<Message<T>, Task<T>> subscription) where T : class
        {

            var describedDelegate = new DescribedDelegate(options, subscription);

            if (!_subscribers.ContainsKey(typeof(T))) return;

            // Get delegates for our type
            var typeDelegates = _subscribers[typeof(T)];

            // Get delegates for type matching our key
            var matchingKey = typeDelegates
                .Where(d => d.Options.Key == options.Key)
                .ToList();

            // Get delegates matching our subscription target
            var matchingDelegates = matchingKey
                .Where(d => d.Subscription.Target.Equals(subscription.Target))
                .ToList();
            
            // Remove
            foreach (var matchingDelegate in matchingDelegates)
            {
                if (typeDelegates.Contains(matchingDelegate))
                {
                    typeDelegates.Remove(matchingDelegate);
                }
            }

            if (typeDelegates.Count == 0)
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
