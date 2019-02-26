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
        
        public PagerOptions Pager { get; set; } = new PagerOptions();
        
        public InfiniteScrollOptions InfiniteScroll { get; set; } = new InfiniteScrollOptions();

        public TEntity Article { get; set; }

        public IPagedResults<TReply> Replies { get; set; }
     
        public EntityOptions Options { get; set; }
        
    }

    [DataContract]
    public class EntityOptions
    {
        
        public string Sort { get; set; } = "CreatedDate";

        public OrderBy Order { get; set; } = OrderBy.Asc;

        public int EntityId { get; set; }

    }

}
