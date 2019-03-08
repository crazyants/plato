using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Reactions.Repositories
{
    public interface IEntityReactionsRepository<TModel> : IRepository<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> SelectEntityReactionsByEntityId(int entityId);

        Task<IEnumerable<TModel>> SelectEntityReactionsByUserIdAndEntityId(int userId, int entityId);
        
    }

}