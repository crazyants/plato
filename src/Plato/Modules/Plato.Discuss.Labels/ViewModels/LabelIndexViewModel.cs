using System.Collections.Generic;
using Plato.Discuss.Labels.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Labels.ViewModels
{
    public class LabelIndexViewModel
    {
        
        public IPagedResults<Label> Results { get; set; }

        public LabelIndexOptions Options { get; set; }

        public PagerOptions Pager { get; set; }

        public IEnumerable<SortColumn> SortColumns { get; set; }

        public IEnumerable<SortOrder> SortOrder { get; set; }
        
    }

    public class LabelIndexOptions
    {
        public string Search { get; set; }
        
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
