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
            string value)
        {

            var model = new SearchIndexViewModel()
            {
                Keywords = value
            };

            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}
