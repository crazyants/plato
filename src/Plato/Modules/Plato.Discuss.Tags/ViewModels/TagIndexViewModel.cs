using System.Collections.Generic;
using Plato.Tags.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Tags.ViewModels
{
    public class TagIndexViewModel
    {
        
        public IPagedResults<Tag> Results { get; set; }

        public TagIndexOptions Options { get; set; }

        public PagerOptions Pager { get; set; }

        public IEnumerable<SortColumn> SortColumns { get; set; }

        public IEnumerable<SortOrder> SortOrder { get; set; }
        
    }

    public class TagIndexOptions
    {
        public string Search { get; set; }

        public ScrollerOptions Scroller { get; set; }

        public SortBy Sort { get; set; } = SortBy.Entities;

        public OrderBy Order { get; set; } = OrderBy.Desc;
        
        public bool EnableEdit { get; set; }

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
