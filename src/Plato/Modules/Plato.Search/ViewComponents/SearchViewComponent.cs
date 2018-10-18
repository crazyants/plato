using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Search.ViewModels;

namespace Plato.Search.ViewComponents
{
    public class SearchViewComponent : ViewComponent
    {

        public SearchViewComponent()
        {

        }

        public Task<IViewComponentResult> InvokeAsync(
            SearchIndexOptions options)
        {

            if (options == null)
            {
                options = new SearchIndexOptions();
            }
            
            return Task.FromResult((IViewComponentResult)View(options));
        }

    }

}
