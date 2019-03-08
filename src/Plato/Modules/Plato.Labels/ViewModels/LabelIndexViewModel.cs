using System.Collections.Generic;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Labels.Models;

namespace Plato.Labels.ViewModels
{
    public class LabelIndexViewModel<TModel> where TModel : class, ILabel
    {
        
        public IPagedResults<TModel> Results { get; set; }

        public LabelIndexOptions Options { get; set; }

        public PagerOptions Pager { get; set; }

        public IEnumerable<SortColumn> SortColumns { get; set; }

        public IEnumerable<SortOrder> SortOrder { get; set; }
        
    }

    public class LabelIndexOptions
    {

        public int FeatureId { get; set; }

        public string Search { get; set; }
        
        public LabelSortBy Sort { get; set; } = LabelSortBy.Auto;

        public OrderBy Order { get; set; } = OrderBy.Desc;
        
        public bool EnableEdit { get; set; }

    }

    public class SortColumn
    {
        public string Text { get; set; }

        public LabelSortBy Value { get; set; }
        
    }
    
    public class SortOrder
    {
        public string Text { get; set; }

        public OrderBy Value { get; set; }

    }

    public enum LabelSortBy {
        Auto,
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
