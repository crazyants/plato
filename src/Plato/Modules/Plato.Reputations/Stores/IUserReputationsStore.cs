using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;
using Plato.Reputations.Models;

namespace Plato.Reputations.Stores
{
    public interface IUserReputationsStore<TModel> : IStore<TModel> where TModel : class
    {
        Task<IEnumerable<IReputation>> GetUserReputationsAsync(int userId);

    }

}
