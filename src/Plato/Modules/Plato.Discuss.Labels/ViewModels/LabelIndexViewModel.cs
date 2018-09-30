using System.Collections.Generic;
using Plato.Discuss.Labels.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Labels.ViewModels
{
    public class LabelIndexViewModel
    {

        private readonly IEnumerable<SortColumn> _defaultSortColumns = new List<SortColumn>()
        {
            new SortColumn()
            {
                Text = "Popular",
                Value = SortBy.Entities
            },
            new SortColumn()
            {
                Text = "Follows",
                Value =  SortBy.Follows
            },
            new SortColumn()
            {
                Text = "Views",
                Value = SortBy.Views
            },
            new SortColumn()
            {
                Text = "First Use",
                Value =  SortBy.Created
            },
            new SortColumn()
            {
                Text = "Last Use",
                Value = SortBy.LastEntity
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


        public IPagedResults<Label> Results { get; }

        public LabelIndexOptions Options { get; set; }

        public PagerOptions Pager { get; set; }

        public IEnumerable<SortColumn> SortColumns { get; set; }

        public IEnumerable<SortOrder> SortOrder { get; set; }


        public LabelIndexViewModel()
        {
        }

        public LabelIndexViewModel(
            IPagedResults<Label> results,
            LabelIndexOptions options,
            PagerOptions pager)
        {
            this.SortColumns = _defaultSortColumns;
            this.SortOrder = _defaultSortOrder;
            this.Results = results;
            this.Options = options;
            this.Pager = pager;
            this.Pager.SetTotal(results?.Total ?? 0);
        }

    }

    public class LabelIndexOptions
    {
        public string Search { get; set; }

        public SortBy Sort { get; set; } = SortBy.Entities;

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

    public enum SortBy {
        Id,
        Name,
        Description,
        SortOrder,
        Entities,
        Follows,
        Views,
        LastEntity,
        Created,
        Modified
    }
}
