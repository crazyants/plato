using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Messaging.Abstractions
{
    public class DescribedDelegate
    {

        public MessageOptions Options { get; set; }

        public Delegate Subscription { get; set; }

        public DescribedDelegate(MessageOptions options, Delegate Subscription)
        {
            this.Options = options;
            this.Subscription = Subscription;
        }

    }

}
