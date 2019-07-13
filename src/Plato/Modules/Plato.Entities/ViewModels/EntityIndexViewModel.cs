using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Entities.ViewModels
{
  
    public class EntityIndexViewModel<TModel> where TModel : class
    {

        public IPagedResults<TModel> Results { get; set; }

        public PagerOptions Pager { get; set; } = new PagerOptions();

        public EntityIndexOptions Options { get; set; } = new EntityIndexOptions();

        public ICollection<SortColumn> SortColumns { get; set; }

        public ICollection<SortOrder> SortOrder { get; set; }

        public ICollection<Filter> Filters { get; set; }

    }

    public class EntityIndexOptions
    {
        
        [DataMember(Name = "search")]
        public string Search { get; set; }

        [DataMember(Name = "feature")]
        public int? FeatureId { get; set; }
        
        [DataMember(Name = "filter")]
        public FilterBy Filter { get; set; } = FilterBy.All;

        [DataMember(Name = "sort")]
        public SortBy Sort { get; set; } = SortBy.LastReply;

        [DataMember(Name = "order")]
        public OrderBy Order { get; set; } = OrderBy.Desc;

        [DataMember(Name = "category")]
        public int CategoryId { get; set; } = -1;

        // ---------------

        [JsonIgnore]
        public int[] CategoryIds { get; set; }

        public int CreatedByUserId { get; set; }

        [JsonIgnore]
        public int LabelId { get; set; }

        [JsonIgnore]
        public int TagId { get; set; }
        
        private IDictionary<string, OrderBy> _sortColumns;

        [JsonIgnore]
        public IDictionary<string, OrderBy> SortColumns => _sortColumns ?? (_sortColumns = new Dictionary<string, OrderBy>());

        public void AddSortColumn(string name, OrderBy order)
        {
            if (!SortColumns.ContainsKey(name))
            {
                SortColumns.Add(name, order);
            }
        }

    }

    public class SortColumn
    {
        public string Text { get; set; }

        public SortBy Value { get; set; }

    }

    public class SortOrder
    {
        public string Text { get; set; }

        public OrderBy Value { get; set; }

    }

    public class Filter
    {
        public string Text { get; set; }

        public FilterBy Value { get; set; }

    }

    public enum SortBy
    {
        LastReply,
        Popular,
        Rank,
        Replies,
        Views,
        Participants,
        Reactions,
        Follows,
        Stars,
        SortOrder,
        Created,
        Modified,
        IsPinned
    }

    public enum FilterBy
    {
        All,
        Started,
        Participated,
        Following,
        Starred,
        Unanswered,
        NoReplies
    }

}
