using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Plato.Discuss.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewModels
{


    public class TopicIndexViewModel
    {
        
        private readonly IEnumerable<SortOption> _defaultSortColumns = new List<SortOption>()
        {
            new SortOption()
            {
                Text = "Last Reply",
                Value = SortBy.LastReply
            },
            new SortOption()
            {
                Text = "Replies",
                Value =  SortBy.Replies
            },
            new SortOption()
            {
                Text = "Views",
                Value = SortBy.Views
            },
            new SortOption()
            {
                Text = "Participants",
                Value =  SortBy.Participants
            },
            new SortOption()
            {
                Text = "Reactions",
                Value =  SortBy.Reactions
            },
            new SortOption()
            {
                Text = "Created",
                Value = SortBy.Created
            },
            new SortOption()
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
        
        public IEnumerable<SortOption> SortColumns { get; set; }

        public IEnumerable<SortOrder> SortOrder { get; set; }

        public TopicIndexViewModel()
        {

        }

        public TopicIndexViewModel(
            IPagedResults<Topic> results,
            TopicIndexOptions options,
            PagerOptions pager)
        {
            this.Results = results;
            this.Options = options;
            this.Pager = pager;
            this.SortColumns = SetSortSelection(_defaultSortColumns, options.Sort);
            this.SortOrder = SetOrderSelection(_defaultSortOrder, options.Order);
            this.Pager.SetTotal(results?.Total ?? 0);
        }

        IEnumerable<SortOption> SetSortSelection(IEnumerable<SortOption> options, SortBy value)
        {
            var output = new List<SortOption>();
            foreach (var option in options)
            {
                if (option.Value == value)
                {
                    option.Selected = true;
                }
                output.Add(option);
            }

            return output;
        }

        IEnumerable<SortOrder> SetOrderSelection(IEnumerable<SortOrder> options, OrderBy value)
        {
            var output = new List<SortOrder>();
            foreach (var option in options)
            {
                if (option.Value == value)
                {
                    option.Selected = true;
                }
                output.Add(option);
            }

            return output;
        }


    }

    [DataContract]
    public class TopicIndexOptions
    {

        [DataMember(Name = "search")]
        public string Search { get; set; }

        [DataMember(Name = "channel")]
        public int ChannelId { get; set; }

        public int LabelId { get; set; }


        [DataMember(Name = "sort")]
        public SortBy Sort { get; set; } = SortBy.LastReply;

        [DataMember(Name = "order")]
        public OrderBy Order { get; set; }= OrderBy.Desc;

        [DataMember(Name = "filter")]
        public Filter Filter { get; set; }

    }

    public class SortOption
    {
        public string Text { get; set; }

        public SortBy Value { get; set; }

        public bool Selected { get; set; }

    }

    public class SortOrder
    {
        public string Text { get; set; }

        public OrderBy Value { get; set; }

        public bool Selected { get; set; }

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

    public enum Filter
    {
        All = 0,
        Unanswered
    }

}
