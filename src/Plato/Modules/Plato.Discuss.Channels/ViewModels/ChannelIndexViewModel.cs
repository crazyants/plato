using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Channels.ViewModels
{
    public class ChannelIndexViewModel
    {

        public IPagedResults<Topic> Results { get; }

        public PagerOptions PagerOpts { get; set; }

        public FilterOptions FilterOpts { get; set; }

        public ChannelIndexViewModel()
        {
        }

        public ChannelIndexViewModel(
            IPagedResults<Topic> results,
            PagerOptions pagerOptions)
        {
            this.Results = results;
            this.PagerOpts = pagerOptions;
            this.PagerOpts.SetTotal(results?.Total ?? 0);
        }
        
    }
}
