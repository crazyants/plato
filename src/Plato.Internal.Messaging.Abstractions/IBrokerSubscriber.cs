using System;

namespace Plato.Internal.Messaging.Abstractions
{

    public interface IBrokerSubscriber : IDisposable
    {

        void Subscribe();

        void Unsubscribe();

    }

}
