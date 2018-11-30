using System.Collections.Generic;
using System.Runtime.Serialization;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Search.ViewModels
{
    public class SearchIndexViewModel
    {
        
        public IPagedResults<Entity> Results { get; set; }

        public PagerOptions Pager { get; set; } = new PagerOptions();

        public SearchIndexOptions Options { get; set; } = new SearchIndexOptions();

        public IEnumerable<SortColumn> SortColumns { get; set; }

        public IEnumerable<SortOrder> SortOrder { get; set; }

        public IEnumerable<Filter> Filters { get; set; }


        public SearchIndexViewModel()
        {

        }
        

    }
    
    public class SearchIndexOptions
    {
        [DataMember(Name = "search")]
        public string Search { get; set; }

        [DataMember(Name = "channel")]
        public int ChannelId { get; set; }
        
        [DataMember(Name = "filter")]
        public FilterBy Filter { get; set; } = FilterBy.All;
        
        [DataMember(Name = "sort")]
        public SortBy Sort { get; set; } = SortBy.LastReply;

        [DataMember(Name = "order")]
        public OrderBy Order { get; set; } = OrderBy.Desc;
        
    }
    
 
    public enum SortBy
    {
        LastReply = 0,
        Replies = 1,
        Views = 2,
        Participants = 3,
        Reactions = 4,
        Created = 5,
        Modified = 6
    }

    public enum FilterBy
    {
        All = 0,
        MyTopics = 1,
        Participated = 2,
        Following = 3,
        Starred = 4,
        Unanswered = 5,
        NoReplies = 6
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


}
