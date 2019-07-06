namespace Plato.Stars.Models
{
    
    public class StarType : IStarType
    {

        public string Name { get; } = "Entity";

        public string SubscribeText { get; } = "Follow";

        public string SubscribeDescription { get; } = "Subscribe to get notified via email when updates are posted...";
        
        public string UnsubscribeText { get; } = "Unsubscribe";

        public string UnsubscribeDescription { get; } = "You are subscribed to updates. Unsubscribe below...";

        public string LoginDescription { get; } = "Login to star";

        public string DenyDescription { get; } = "Your don't have permission to star";

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

        public StarType(
            string name,
            string subscribeText,
            string subscribeDescription,
            string unsubscribeText,
            string unsubscribeDescription,
            string loginDescription) : this(name, subscribeText, subscribeDescription, unsubscribeText, unsubscribeDescription)
        {
            this.LoginDescription = loginDescription;
        }
        
        public StarType(
            string name,
            string subscribeText,
            string subscribeDescription,
            string unsubscribeText,
            string unsubscribeDescription,
            string loginDescription,
            string denyDescription) : this(name, subscribeText, subscribeDescription, unsubscribeText, unsubscribeDescription, loginDescription)
        {
            this.DenyDescription = denyDescription;
        }

    }

}
