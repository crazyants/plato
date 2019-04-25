using System;
using System.Collections.Generic;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Reporting.ViewModels
{
    public class ReportIndexViewModel<TModel> where TModel : class
    {

        public IPagedResults<TModel> Results { get; set; }

        public PagerOptions Pager { get; set; }

        public ReportIndexOptions Options { get; set; }

        public ICollection<SortColumn> SortColumns { get; set; }

        public ICollection<SortOrder> SortOrder { get; set; }

        public ICollection<Filter> Filters { get; set; }


    }

    public class ReportIndexOptions
    { 

        public DateTimeOffset StartDate { get; set; } = DateTimeOffset.UtcNow.AddDays(-7);
        
        public DateTimeOffset EndDate { get; set; } = DateTimeOffset.UtcNow;

        public FilterBy Filter { get; set; } = FilterBy.All;

        public int FeatureId { get; set; }

        public string Search { get; set; }

        public SortBy Sort { get; set; }

        public OrderBy Order { get; set; } = OrderBy.Desc;

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
        Auto = 0,
        Rank = 1,
        LastReply = 2,
        Replies = 3,
        Views = 4,
        Participants = 5,
        Reactions = 6,
        Follows = 7,
        Stars = 8,
        SortOrder = 9,
        Created = 10,
        Modified = 11
    }

    public enum FilterBy
    {
        All = 0,
        Started = 1,
        Participated = 2,
        Following = 3,
        Starred = 4,
        Unanswered = 5,
        NoReplies = 6
    }


}
