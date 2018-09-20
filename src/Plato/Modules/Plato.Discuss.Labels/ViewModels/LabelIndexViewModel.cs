using Plato.Discuss.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;
using Plato.Discuss.ViewModels;

namespace Plato.Discuss.Labels.ViewModels
{
    public class LabelIndexViewModel
    {

        public IPagedResults<Topic> Results { get; }

        public PagerOptions PagerOpts { get; set; }

        public TopicIndexOptions TopicIndexOpts { get; set; }

        public LabelIndexViewModel()
        {
        }

        public LabelIndexViewModel(
            IPagedResults<Topic> results,
            PagerOptions pagerOptions)
        {
            this.Results = results;
            this.PagerOpts = pagerOptions;
            this.PagerOpts.SetTotal(results?.Total ?? 0);
        }
        
    }

    public class ViewOptions
    {
        public int LabelId { get; set; }
    }

}
