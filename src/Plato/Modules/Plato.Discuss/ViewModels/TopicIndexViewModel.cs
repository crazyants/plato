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

        private readonly IEnumerable<Filter> _defaultFilters = new List<Filter>()
        {
            new Filter()
            {
                Text = "All",
                Value = FilterBy.All
            },
            new Filter()
            {
                Text = "-" // represents menu divider
            },
            new Filter()
            {
                Text = "My Topics",
                Value = FilterBy.MyTopics
            },
            new Filter()
            {
                Text = "Participated",
                Value = FilterBy.Participated
            },
            new Filter()
            {
                Text = "Following",
                Value = FilterBy.Following
            },
            new Filter()
            {
                Text = "Starred",
                Value = FilterBy.Starred
            },
            new Filter()
            {
                Text = "-"  // represents menu divider
            },
            new Filter()
            {
                Text = "Unanswered",
                Value = FilterBy.Unanswered
            },
            new Filter()
            {
                Text = "No Replies",
                Value = FilterBy.NoReplies
            }
        };
        
        private readonly IEnumerable<SortColumn> _defaultSortColumns = new List<SortColumn>()
        {
            new SortColumn()
            {
                Text = "Last Reply",
                Value = SortBy.LastReply
            },
            new SortColumn()
            {
                Text = "Replies",
                Value =  SortBy.Replies
            },
            new SortColumn()
            {
                Text = "Views",
                Value = SortBy.Views
            },
            new SortColumn()
            {
                Text = "Participants",
                Value =  SortBy.Participants
            },
            new SortColumn()
            {
                Text = "Reactions",
                Value =  SortBy.Reactions
            },
            new SortColumn()
            {
                Text = "Created",
                Value = SortBy.Created
            },
            new SortColumn()
            {
                Text = "Modified",
                Value = SortBy.Modified
            }
        };

        private readonly IEnumerable<SortOrder> _defaultSortOrder = new List<SortOrder>()
        {
            new SortOrder()
            {
                Text = "Descending",
                Value = OrderBy.Desc
            },
            new SortOrder()
            {
                Text = "Ascending",
                Value = OrderBy.Asc
            },
        };

        public IPagedResults<Topic> Results { get; }

        public PagerOptions Pager { get; set; }
        
        public TopicIndexOptions Options { get; set; }
        
        public IEnumerable<SortColumn> SortColumns { get; set; }

        public IEnumerable<SortOrder> SortOrder { get; set; }

        public IEnumerable<Filter> Filters { get; set; }

        public TopicIndexViewModel()
        {

        }

        public TopicIndexViewModel(
            IPagedResults<Topic> results,
            TopicIndexOptions options,
            PagerOptions pager)
        {
            this.SortColumns = _defaultSortColumns;
            this.SortOrder = _defaultSortOrder;
            this.Filters = _defaultFilters;
            this.Results = results;
            this.Options = options;
            this.Pager = pager;
            this.Pager.SetTotal(results?.Total ?? 0);
        }

    }

    public class TopicIndexParams
    {

        [DataMember(Name = "channel")]
        public int ChannelId { get; set; }
        
        public int CreatedByUserId { get; set; }

        public int LabelId { get; set; }

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

        public TopicIndexOptions(RouteData routeData)
        {
            Search = GetRouteValueOrDefault<string>("opts.earch", routeData, Search);
            //ChannelId = GetRouteValueOrDefault<int>("opts.channelId", routeData, ChannelId);
            //LabelId = GetRouteValueOrDefault<int>("opts.labelId", routeData, LabelId);
            Sort = GetRouteValueOrDefault<SortBy>("opts.sort", routeData, Sort);
            Order = GetRouteValueOrDefault<OrderBy>("opts.order", routeData, Order);
        }

        public string BuildSortOrderByHref(RouteData routeData, UrlHelper urlHelper, OrderBy order)
        {
            var data = new RouteValueDictionary(routeData.Values);
            if (order != this.Order)
            {
                data.Add("opts.order", order);
            }

            return urlHelper.RouteUrl(new UrlRouteContext { Values = data });


        }

        private T GetRouteValueOrDefault<T>(string key, RouteData routeData, T defaultValue)
        {

            if (routeData == null)
            {
                return defaultValue;
            }

            var found = routeData.Values.TryGetValue(key, out object value);
            if (found)
            {
                return (T) value;
            }

            return defaultValue;
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
