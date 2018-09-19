using System;
using System.Collections.Generic;
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
                Text = "Replies",
                Value = "replies"
            },
            new SortOption()
            {
                Text = "Views",
                Value = "views"
            },
            new SortOption()
            {
                Text = "Participants",
                Value = "participants"
            },
            new SortOption()
            {
                Text = "Reactions",
                Value = "reactions"
            },
            new SortOption()
            {
                Text = "Created",
                Value = "created"
            },
            new SortOption()
            {
                Text = "LastPost",
                Value = "lastpost"
            }
        };

        private readonly IEnumerable<SortOption> _defaultSortOrder = new List<SortOption>()
        {
            new SortOption()
            {
                Text = "Descending",
                Value = OrderBy.Desc.ToString()
            },
            new SortOption()
            {
                Text = "Ascending",
                Value = OrderBy.Asc.ToString()
            },
        };

        public IPagedResults<Topic> Results { get; }

        public PagerOptions PagerOpts { get; set; }
        
        public ViewOptions ViewOpts { get; set; }
        
        public IEnumerable<SortOption> SortColumns { get; set; }

        public IEnumerable<SortOption> SortOrder { get; set; }

        public TopicIndexViewModel()
        {

        }

        public TopicIndexViewModel(
            IPagedResults<Topic> results,
            ViewOptions viewOptions,
            PagerOptions pagerOptions)
        {
            this.Results = results;
            this.ViewOpts = viewOptions;
            this.PagerOpts = pagerOptions;
            this.SortColumns = SetSelection(_defaultSortColumns, viewOptions.Sort);
            this.SortOrder = SetSelection(_defaultSortOrder, viewOptions.Order.ToString());
            this.PagerOpts.SetTotal(results?.Total ?? 0);
        }

        IEnumerable<SortOption> SetSelection(IEnumerable<SortOption> options, string value)
        {
            var output = new List<SortOption>();
            foreach (var option in options)
            {
                if (option.Value.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    option.Selected = true;
                }
                output.Add(option);
            }

            return output;
        }

    }

    public class ViewOptions
    {
        public string Search { get; set; }

        public int ChannelId { get; set; }

        public string Sort { get; set; }

        public OrderBy Order { get; set; } 

    }

    public class SortOption
    {
        public string Text { get; set; }

        public string Value { get; set; }

        public bool Selected { get; set; }

    }

}
