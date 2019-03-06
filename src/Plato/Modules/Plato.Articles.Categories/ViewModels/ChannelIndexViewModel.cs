using Plato.Articles.Models;
using Plato.Articles.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Categories.ViewModels
{
    public class ChannelIndexViewModel
    {

        public IPagedResults<Article> Results { get; }

        public PagerOptions PagerOpts { get; set; }

        public ChannelIndexOptions ChannelIndexOpts { get; set; }

        public EntityIndexOptions TopicIndexOpts { get; set; }
        
        public ChannelIndexViewModel()
        {
        }

        public ChannelIndexViewModel(
            IPagedResults<Article> results,
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
