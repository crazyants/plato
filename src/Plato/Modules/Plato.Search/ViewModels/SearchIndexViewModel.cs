using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Search.ViewModels
{
    public class SearchIndexViewModel
    {

        public IPagedResults<Entity> Results { get; }

        public PagerOptions PagerOpts { get; set; } = new PagerOptions();

        public ViewOptions ViewOpts { get; set; } = new ViewOptions();
        
        public SearchIndexViewModel()
        {

        }

        public SearchIndexViewModel(
            IPagedResults<Entity> results,
            ViewOptions viewOptions,
            PagerOptions pagerOptions)
        {
            this.Results = results;
            this.ViewOpts = viewOptions;
            this.PagerOpts = pagerOptions;
            this.PagerOpts.SetTotal(results?.Total ?? 0);
        }

    }


    public class ViewOptions
    {
        public string Search { get; set; }

        public int ChannelId { get; set; }

        public EntityOrder Order { get; set; }

    }


    public enum EntityOrder
    {
        Username,
        Email
    }

}
