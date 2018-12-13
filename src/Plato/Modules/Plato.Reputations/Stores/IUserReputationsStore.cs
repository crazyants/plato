using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Reputations.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Reputations.Stores
{
    public interface IUserReputationsStore<TModel> : IStore<TModel> where TModel : class
    {
        Task<IEnumerable<IReputation>> GetUserReputationsAsync(int userId);

    }

}
