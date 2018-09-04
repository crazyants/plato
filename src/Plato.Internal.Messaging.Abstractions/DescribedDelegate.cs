using System;

namespace Plato.Internal.Messaging.Abstractions
{
    public class DescribedDelegate : IEquatable<DescribedDelegate>
    {
        public MessageOptions Options { get; set; }

        public Delegate Subscription { get; set; }

        public string Id { get; }

        public DescribedDelegate(MessageOptions options, Delegate subscription)
        {
            this.Options = options;
            this.Subscription = subscription;
            this.Id = options.Key + "_" + System.Guid.NewGuid().ToString();
        }
        
        public bool Equals(DescribedDelegate other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != this.GetType()) return false;
            return other.Id == this.Id;
        }
    }

}
