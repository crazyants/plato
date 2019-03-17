using System.Collections.Generic;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Tags;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Tags.ViewModels
{
    public class TagIndexViewModel<TModel> where TModel : class, ITag
    {
        
        public IPagedResults<TModel> Results { get; set; }

        public TagIndexOptions Options { get; set; }

        public PagerOptions Pager { get; set; }

        public IEnumerable<SortColumn> SortColumns { get; set; }

        public IEnumerable<SortOrder> SortOrder { get; set; }
        
    }

    public class TagIndexOptions
    {

        public int FeatureId { get; set; }

        public string Search { get; set; }
        
        public TagSortBy Sort { get; set; } = TagSortBy.Auto;

        public OrderBy Order { get; set; } = OrderBy.Desc;
        
        public bool EnableEdit { get; set; }

    }

    public class SortColumn
    {
        public string Text { get; set; }

        public TagSortBy Value { get; set; }
        
    }
    
    public class SortOrder
    {
        public string Text { get; set; }

        public OrderBy Value { get; set; }

    }

    public enum TagSortBy {
        Auto,
        Id,
        Name,
        Description,
        SortOrder,
        Entities,
        Follows,
        LastEntity,
        Created,
        Modified
    }

}
