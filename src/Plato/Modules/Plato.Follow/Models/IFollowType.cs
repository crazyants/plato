namespace Plato.Follow.Models
{
    public interface IFollowType
    {
        string Name { get; }

        string SubscribeText { get; }

        string SubscribeDescription { get; }

        string UnsubscribeText { get; }

        string UnsubscribeDescription { get; }

    }

}
