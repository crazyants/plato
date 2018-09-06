using System;

namespace Plato.Internal.Messaging.Abstractions
{

    public interface IBrokerSubscriber 
    {

        void Subscribe();

        void Unsubscribe();

    }

}
