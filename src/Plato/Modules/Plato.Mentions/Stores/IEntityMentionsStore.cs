using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Mentions.Stores
{

    public interface IEntityMentionsStore<TModel> : IStore<TModel> where TModel : class
    {
        Task<bool> DeleteByEntityIdAsync(int entityId);

        Task<bool> DeleteByEntityReplyIdAsync(int entityReplyId);

    }

}
