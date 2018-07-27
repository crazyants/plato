using Plato.Discuss.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewModels
{
    public class HomeTopicViewModel
    {

        public HomeTopicViewModel()
        {

        }

        public HomeTopicViewModel(
            IPagedResults<Reply> results,
            PagerOptions pagerOptions)
        {
            this.Results = results;
            this.PagerOpts = pagerOptions;
            this.PagerOpts.SetTotal(results?.Total ?? 0);
        }

        public PagerOptions PagerOpts { get; set; }

        public IPagedResults<Reply> Results { get; set; }
        
        public FilterOptions FilterOpts { get; set; }
        
        public Topic Entity { get; set; }
        
    }

}
