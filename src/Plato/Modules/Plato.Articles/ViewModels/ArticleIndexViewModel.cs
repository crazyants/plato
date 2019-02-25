using System.Collections.Generic;
using System.Runtime.Serialization;
using Plato.Articles.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.ViewModels
{
    
    public class ArticleIndexViewModel
    {

        public IPagedResults<Article> Results { get; set; }

        public PagerOptions Pager { get; set; }
        
        public ArticleIndexOptions Options { get; set; }
        
        public IEnumerable<SortColumn> SortColumns { get; set; }

        public IEnumerable<SortOrder> SortOrder { get; set; }

        public IEnumerable<Filter> Filters { get; set; }

        public ArticleIndexViewModel()
        {
        }
        
    }

    [DataContract]
    public class ArticleIndexOptions
    {
  
        [DataMember(Name = "search")]
        public string Search { get; set; }
        
        [DataMember(Name = "sort")]
        public SortBy Sort { get; set; } = SortBy.LastReply;

        [DataMember(Name = "order")]
        public OrderBy Order { get; set; } = OrderBy.Desc;

        [DataMember(Name = "filter")]
        public FilterBy Filter { get; set; } = FilterBy.All;

        public bool EnableCard { get; set; } = true;

        public ArticleIndexParams Params { get; set; }
        
        public InfiniteScrollOptions InfiniteScroll { get; set; }

        public ArticleIndexOptions()
        {
            Params = new ArticleIndexParams();
            InfiniteScroll = new InfiniteScrollOptions();
        }
        
    }

    public class ArticleIndexParams
    {

        [DataMember(Name = "channel")]
        public int ChannelId { get; set; }

        public int[] ChannelIds { get; set; }


        public int CreatedByUserId { get; set; }

        public int LabelId { get; set; }

        public int TagId { get; set; }

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
        Replies,
        Views,
        Participants,
        Reactions,
        Follows,
        Stars,
        Created,
        Modified
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

}
