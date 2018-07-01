using System;

namespace Plato.Internal.Messaging.Abstractions
{
    public class Message<T> where T : class
    {

        public object Who { get; private set; }

        public T What { get; set; }

        public DateTime When { get; private set; }

        public Message(T payload, object source)
        {
            this.Who = source;
            this.What = payload;
            this.When = DateTime.UtcNow;
        }
    }


}
