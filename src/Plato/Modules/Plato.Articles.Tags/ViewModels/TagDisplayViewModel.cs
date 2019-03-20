using Plato.Articles.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Entities.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Tags.ViewModels
{
    // TODO refactor into Plato.Tags
    public class TagDisplayViewModel
    {

        public IPagedResults<Article> Results { get; }

        public PagerOptions Pager { get; set; }

        public EntityIndexOptions Options { get; set; }

        public TagDisplayViewModel()
        {
        }

        public TagDisplayViewModel(
            IPagedResults<Article> results,
            PagerOptions pagerOptions)
        {
            this.Results = results;
            this.Pager = pagerOptions;
            this.Pager.SetTotal(results?.Total ?? 0);
        }
        
    }
    
}
