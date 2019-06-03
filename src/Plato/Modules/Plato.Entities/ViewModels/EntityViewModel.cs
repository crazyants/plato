using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
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

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "alias")]
        public string Alias { get; set; }

        [DataMember(Name = "replyId")]
        public int ReplyId { get; set; }

        [DataMember(Name = "sort")]
        public string Sort { get; set; } = "CreatedDate";

        [DataMember(Name = "order")]
        public OrderBy Order { get; set; } = OrderBy.Asc;

        // -----------------


        private IDictionary<string, OrderBy> _sortColumns;

        [JsonIgnore]
        public IDictionary<string, OrderBy> SortColumns
        {
            get => _sortColumns ?? (_sortColumns = new Dictionary<string, OrderBy>());
            set => _sortColumns = value;
        }

    }

}
