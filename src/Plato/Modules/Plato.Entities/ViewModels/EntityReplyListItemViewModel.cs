using Plato.Entities.Models;

namespace Plato.Entities.ViewModels
{
    public class EntityReplyListItemViewModel<TEntity, TReply>
        where TEntity : class, IEntity
        where TReply : class, IEntityReply
    {

        public TReply Reply { get; set; }

        public TEntity Entity { get; set; }
        
    }

}
