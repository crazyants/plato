using Plato.Discuss.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Entities.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Tags.ViewModels
{
    public class TagDisplayViewModel
    {

        public IPagedResults<Topic> Results { get; }

        public PagerOptions Pager { get; set; }

        public EntityIndexOptions Options { get; set; }

        public TagDisplayViewModel()
        {
        }

        public TagDisplayViewModel(
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
