namespace Plato.Stars.Models
{
    public interface IStarType
    {
        string Name { get; }

        string SubscribeText { get; }

        string SubscribeDescription { get; }

        string UnsubscribeText { get; }

        string UnsubscribeDescription { get; }

        string LoginDescription { get; }

        string DenyDescription { get; }
    }

}
