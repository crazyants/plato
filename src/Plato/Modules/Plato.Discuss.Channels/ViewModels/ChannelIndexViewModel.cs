using Plato.Discuss.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Channels.ViewModels
{
    public class ChannelIndexViewModel
    {

        public IPagedResults<Topic> Results { get; }

        public PagerOptions PagerOpts { get; set; }

        public ViewOptions ChannelViewOpts { get; set; }

        public Discuss.ViewModels.ViewOptions TopicViewOpts { get; set; }
        
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

    public class ViewOptions
    {

        public int ChannelId { get; set; }

        public bool EnableEdit { get; set; }

    }
}
