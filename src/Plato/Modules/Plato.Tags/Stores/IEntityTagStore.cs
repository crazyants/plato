using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Tags.Stores
{
    public interface IEntityTagStore<TModel> : IStore2<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> GetByEntityId(int entityId);

        Task<IEnumerable<TModel>> GetByEntityReplyId(int entityReplyId);

        Task<bool> DeleteByEntityId(int entityId);

        Task<bool> DeleteByEntityIdAndTagIdId(int entityId, int tagId);

    }


}
