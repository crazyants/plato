namespace Plato.Stars.Models
{
    
    public class StarType : IStarType
    {

        public string Name { get; } = "Entity";

        public string SubscribeText { get; } = "Follow";

        public string SubscribeDescription { get; } = "Subscribe to get notified via email when updates are posted...";
        
        public string UnsubscribeText { get; } = "Unsubscribe";

        public string UnsubscribeDescription { get; } = "You are subscribed to updates. Unsubscribe below...";

        public StarType()
        {

        }

        public StarType(string name)
        {
            this.Name = name;
        }

        public StarType(
            string name,
            string subscribeText) : this(name)
        {
            this.SubscribeText = subscribeText;
        }

        public StarType(
            string name,
            string subscribeText,
            string subscribeDescription) : this(name, subscribeText)
        {
            this.SubscribeDescription = subscribeDescription;
        }

        public StarType(
            string name,
            string subscribeText,
            string subscribeDescription,
            string unsubscribeText) : this(name, subscribeText, subscribeDescription)
        {
            this.UnsubscribeText = unsubscribeText;
        }

        public StarType(
            string name,
            string subscribeText,
            string subscribeDescription,
            string unsubscribeText,
            string unsubscribeDescription) : this(name, subscribeText, subscribeDescription, unsubscribeText)
        {
            this.UnsubscribeDescription = unsubscribeDescription;
        }


    }

}
