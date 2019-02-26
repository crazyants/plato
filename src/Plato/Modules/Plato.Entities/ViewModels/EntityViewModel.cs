using System.Runtime.Serialization;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Entities.ViewModels
{
    public class EntityViewModel<TEntity, TReply> 
        where TEntity : class, IEntity
        where TReply : class, IEntityReply
    {
        
        public TEntity Entity { get; set; }

        public IPagedResults<TReply> Replies { get; set; }

        public PagerOptions Pager { get; set; } = new PagerOptions();

        public EntityOptions Options { get; set; }
        
    }

    [DataContract]
    public class EntityOptions
    {

        public int EntityId { get; set; }

        public string Sort { get; set; } = "CreatedDate";

        public OrderBy Order { get; set; } = OrderBy.Asc;
        
    }

}
