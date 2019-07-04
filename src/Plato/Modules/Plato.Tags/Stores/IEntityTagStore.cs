using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Tags.Stores
{
    public interface IEntityTagStore<TModel> : IStore<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> GetByEntityIdAsync(int entityId);

        Task<IEnumerable<TModel>> GetByEntityReplyIdAsync(int entityReplyId);

        Task<bool> DeleteByEntityIdAsync(int entityId);

        Task<bool> DeleteByEntityIdAndTagIdIdAsync(int entityId, int tagId);

    }


}
