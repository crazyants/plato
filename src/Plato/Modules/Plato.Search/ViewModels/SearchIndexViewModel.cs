using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Search.ViewModels
{
    public class SearchIndexViewModel
    {

        public IPagedResults<Entity> Results { get; }

        public PagerOptions Pager { get; set; } = new PagerOptions();

        public SearchIndexOptions Options { get; set; } = new SearchIndexOptions();
        
        public SearchIndexViewModel()
        {

        }
        public SearchIndexViewModel(
            SearchIndexOptions options,
            PagerOptions pager)
        {
            this.Options = options;
            this.Pager = pager;
        }

        public SearchIndexViewModel(
            IPagedResults<Entity> results,
            SearchIndexOptions options,
            PagerOptions pager)
        {
            this.Results = results;
            this.Options = options;
            this.Pager = pager;
            this.Pager.SetTotal(results?.Total ?? 0);
        }

    }


    public class SearchIndexOptions
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
