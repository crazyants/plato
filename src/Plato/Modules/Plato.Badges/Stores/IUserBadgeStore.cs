using Plato.Internal.Stores.Abstractions;

namespace Plato.Badges.Stores
{
    public interface IUserBadgeStore<TModel> : IStore<TModel> where TModel : class
    {
    }

}
