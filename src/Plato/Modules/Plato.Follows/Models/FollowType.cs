namespace Plato.Follows.Models
{
    
    public class FollowType : IFollowType
    {

        public string Name { get; } = "Entity";

        public string SubscribeText { get; } = "Follow";

        public string SubscribeDescription { get; } = "Subscribe to get notified via email when updates are posted...";
        
        public string UnsubscribeText { get; } = "Unsubscribe";

        public string UnsubscribeDescription { get; } = "You are subscribed to updates. Unsubscribe below...";

        public string LoginDescription { get; } = "Login to follow";

        public string DenyDescription { get; } = "You don't have permission to follow";
        

        public FollowType()
        {

        }

        public FollowType(string name)
        {
            this.Name = name;
        }

        public FollowType(
            string name,
            string subscribeText) : this(name)
        {
            this.SubscribeText = subscribeText;
        }

        public FollowType(
            string name,
            string subscribeText,
            string subscribeDescription) : this(name, subscribeText)
        {
            this.SubscribeDescription = subscribeDescription;
        }

        public FollowType(
            string name,
            string subscribeText,
            string subscribeDescription,
            string unsubscribeText) : this(name, subscribeText, subscribeDescription)
        {
            this.UnsubscribeText = unsubscribeText;
        }

        public FollowType(
            string name,
            string subscribeText,
            string subscribeDescription,
            string unsubscribeText,
            string unsubscribeDescription) : this(name, subscribeText, subscribeDescription, unsubscribeText)
        {
            this.UnsubscribeDescription = unsubscribeDescription;
        }


        public FollowType(
            string name,
            string subscribeText,
            string subscribeDescription,
            string unsubscribeText,
            string unsubscribeDescription,
            string loginDescription) : this(name, subscribeText, subscribeDescription, unsubscribeText, unsubscribeDescription)
        {
            this.LoginDescription = loginDescription;
        }

        public FollowType(
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
