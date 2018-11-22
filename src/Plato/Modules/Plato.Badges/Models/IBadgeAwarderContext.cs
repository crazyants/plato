namespace Plato.Badges.Models
{
    public interface IBadgeAwarderContext<TModel> where TModel : class
    {

        TModel Model { get; set; }

        IBadge Badge { get; set; }

    }

}
