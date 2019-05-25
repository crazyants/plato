using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Entities.Reactions.Repositories
{
    public interface IEntityReactionsRepository<TModel> : IRepository2<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> SelectEntityReactionsByEntityId(int entityId);

        Task<IEnumerable<TModel>> SelectEntityReactionsByUserIdAndEntityId(int userId, int entityId);
        
    }

}