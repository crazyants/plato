using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plato.Internal.Messaging.Abstractions
{
    public interface IBroker : IDisposable
    {

        Task<IEnumerable<Func<Message<T>, Task<T>>>> Pub<T>(object source, T message) where T : class;

        void Sub<T>(Action<Message<T>> subscription) where T : class;

        void Sub<T>(Func<Message<T>, Task<T>> subscription) where T : class;

        void Unsub<T>(Action<Message<T>> subscription) where T : class;

        void Unsub<T>(Func<Message<T>, Task<T>> subscription) where T : class;

    }

}
