using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plato.Internal.Messaging.Abstractions
{
    public interface IBroker : IDisposable
    {

        void Pub<T>(object source, T message) where T : class;

        void Sub<T>(Action<Message<T>> subscription) where T : class;

        void Unsub<T>(Action<Message<T>> subscription) where T : class;

        Task<IEnumerable<Func<Message<T>, Task<T>>>> Pull<T>(object source, T message) where T : class;

        void Push<T>(Func<Message<T>, Task<T>> subscription) where T : class;

        void Pop<T>(Action<Message<T>> subscription) where T : class;

    }

}
