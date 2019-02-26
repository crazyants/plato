using Plato.Discuss.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Channels.ViewModels
{
    public class ChannelIndexViewModel
    {

        public IPagedResults<Topic> Results { get; }

        public PagerOptions PagerOpts { get; set; }

        public ChannelIndexOptions ChannelIndexOpts { get; set; }

        public EntityIndexOptions TopicIndexOpts { get; set; }
        
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

    public class ChannelIndexOptions
    {

        public int ChannelId { get; set; }

        public bool EnableEdit { get; set; }

    }
}
