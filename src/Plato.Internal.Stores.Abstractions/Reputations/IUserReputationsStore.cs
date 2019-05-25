using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Reputations;

namespace Plato.Internal.Stores.Abstractions.Reputations
{
    public interface IUserReputationsStore<TModel> : IStore2<TModel> where TModel : class
    {
        Task<IEnumerable<IReputation>> GetUserReputationsAsync(int userId, IEnumerable<IReputation> reputations);

    }

}
