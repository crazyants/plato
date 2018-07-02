using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Internal.Messaging.Extensions
{
    public static class BrokerExtensions
    {

        public static async Task<IEnumerable<Func<Message<T>, Task<T>>>> Pub<T>(
            this IBroker broker,
            object sender, 
            string key,
            T value) where T : class
        {
            return await broker.Pub<T>(sender, new MessageOptions()
            {
                Key = key
            }, value);
        }

    }
}
