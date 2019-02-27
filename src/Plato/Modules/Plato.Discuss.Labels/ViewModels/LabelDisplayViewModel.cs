using Plato.Discuss.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Entities.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Labels.ViewModels
{
    public class LabelDisplayViewModel
    {

        public IPagedResults<Topic> Results { get; }

        public PagerOptions Pager { get; set; }

        public EntityIndexOptions Options { get; set; }

        public LabelDisplayViewModel()
        {
        }

        public LabelDisplayViewModel(
            IPagedResults<Topic> results,
            PagerOptions pagerOptions)
        {
            this.Results = results;
            this.Pager = pagerOptions;
            this.Pager.SetTotal(results?.Total ?? 0);
        }
        
    }

    public class ViewOptions
    {
        public int LabelId { get; set; }
    }

}
