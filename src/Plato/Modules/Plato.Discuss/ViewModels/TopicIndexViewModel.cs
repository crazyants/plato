using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Plato.Discuss.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewModels
{
    
    public class TopicIndexViewModel
    {

        public IPagedResults<Topic> Results { get; set; }

        public PagerOptions Pager { get; set; }
        
        public TopicIndexOptions Options { get; set; }
        
        public IEnumerable<SortColumn> SortColumns { get; set; }

        public IEnumerable<SortOrder> SortOrder { get; set; }

        public IEnumerable<Filter> Filters { get; set; }

        public TopicIndexViewModel()
        {
        }
        
    }

    public class TopicIndexParams
    {

        [DataMember(Name = "channel")]
        public int ChannelId { get; set; }
        
        public int CreatedByUserId { get; set; }

        public int LabelId { get; set; }

        public int TagId { get; set; }

    }
    
    [DataContract]
    public class TopicIndexOptions
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

        public TopicIndexParams Params { get; set; } = new TopicIndexParams();
        
        public TopicIndexOptions()
        {
            Params = new TopicIndexParams();
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

}
