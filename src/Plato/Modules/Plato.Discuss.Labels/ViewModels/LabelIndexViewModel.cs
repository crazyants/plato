using Plato.Discuss.Labels.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Labels.ViewModels
{
    public class LabelIndexViewModel
    {

        public IPagedResults<Label> Results { get; }

        public LabelIndexOptions Options { get; set; }

        public PagerOptions Pager { get; set; }
        
        public LabelIndexViewModel()
        {
        }

        public LabelIndexViewModel(
            IPagedResults<Label> results,
            LabelIndexOptions options,
            PagerOptions pager)
        {
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
